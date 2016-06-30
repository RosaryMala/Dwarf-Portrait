using DFHack;
using dfproto;
using RemoteFortressReader;
using System.Collections.Generic;

namespace Dwarf_Portrait
{
    class DFConnection
    {
        // Remote bindings
        private RemoteFunction<EmptyMessage, UnitList> unitListCall;
        private RemoteFunction<EmptyMessage, CreatureRawList> creatureRawListCall;
        private RemoteFunction<EmptyMessage, MaterialList> materialListCall;
        private color_ostream dfNetworkOut = new color_ostream();
        private RemoteClient networkClient;

        public UnitList unitList;
        public static CreatureRawList creatureRawList;
        static MaterialList materialRawList;
        public static Dictionary<MatPairStruct, MaterialDefinition> MaterialRaws { get; private set; }

        /// <summary>
        /// Tries to bind an RPC function, leaving returning null if it fails.
        /// </summary>
        /// <typeparam name="Input">Protobuf class used as an input</typeparam>
        /// <param name="client">Connection to Dwarf Fortress</param>
        /// <param name="name">Name of the RPC function to bind to</param>
        /// <param name="proto">Name of the protobuf file to use</param>
        /// <returns>Bound remote function on success, otherwise null.</returns>
        RemoteFunction<Input> CreateAndBind<Input>(RemoteClient client, string name, string proto = "") where Input : class, ProtoBuf.IExtensible, new()
        {
            RemoteFunction<Input> output = new RemoteFunction<Input>();
            if (output.bind(client, name, proto))
                return output;
            else
                return null;
        }

        /// <summary>
        /// Tries to bind an RPC function, returning null if it fails.
        /// </summary>
        /// <typeparam name="Input">Protobuf class used as an input</typeparam>
        /// <typeparam name="Output">Protobuf class to use as an output</typeparam>
        /// <param name="client">Connection to Dwarf Fortress</param>
        /// <param name="name">Name of the RPC function to bind to</param>
        /// <param name="proto">Name of the protobuf file to use</param>
        /// <returns>Bound remote function on success, otherwise null.</returns>
        RemoteFunction<Input, Output> CreateAndBind<Input, Output>(RemoteClient client, string name, string proto = "")
            where Input : class, ProtoBuf.IExtensible, new()
            where Output : class, ProtoBuf.IExtensible, new()
        {
            RemoteFunction<Input, Output> output = new RemoteFunction<Input, Output>();
            if (output.bind(client, name, proto))
                return output;
            else
                return null;
        }

        /// <summary>
        /// Bind the RPC functions we'll be calling
        /// </summary>
        void BindMethods()
        {
            unitListCall = CreateAndBind<EmptyMessage, UnitList>(networkClient, "GetUnitList", "RemoteFortressReader");
            creatureRawListCall = CreateAndBind<EmptyMessage, CreatureRawList>(networkClient, "GetCreatureRaws", "RemoteFortressReader");
            materialListCall = CreateAndBind<EmptyMessage, MaterialList>(networkClient, "GetMaterialList", "RemoteFortressReader");
        }

        /// <summary>
        /// Connect to DF, fetch initial data, start things running
        /// </summary>
        public void ConnectAndFetch()
        {
            networkClient = new DFHack.RemoteClient(dfNetworkOut);
            bool success = networkClient.connect();
            if (!success)
            {
                networkClient.disconnect();
                networkClient = null;
                return;
            }
            BindMethods();

            if(creatureRawListCall != null)
            {
                creatureRawListCall.execute(null, out creatureRawList);
            }
            if(unitListCall != null)
            {
                unitListCall.execute(null, out unitList);
            }
            if(materialListCall != null)
            {
                materialListCall.execute(null, out materialRawList);

                MaterialRaws = new Dictionary<MatPairStruct, MaterialDefinition>();

                foreach (var item in materialRawList.material_list)
                {
                    MaterialRaws[item.mat_pair] = item;
                }
            }

            networkClient.disconnect();
        }
    }
}
