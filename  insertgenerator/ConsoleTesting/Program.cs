using System;
using System.Text;
using System.Collections.Generic;
using Suru.Common.EncryptionLibrary;

namespace Suru.InsertGenerator.ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test_Asymetric_Encryption_Class();
            //Test_Symetric_Encryption_Class();
            //Test_Salt_and_Pass_Generator();
            Test_Ordering_Table_List();

            Console.WriteLine();
            Console.WriteLine("Press ENTER key to close.");
            Console.ReadLine();
        }

        //Test Symetric encryption / decryption features
        private static void Test_Symetric_Encryption_Class()
        {
            //El método soporta más de 255 caracteres...
            String TextToEncrypt = "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345";

            Console.WriteLine("Text to process: " + TextToEncrypt);

            String EncryptedText = Encryption.SymetricEncrypt(TextToEncrypt);

            Console.WriteLine("Symetric Encrypted text: " + EncryptedText);

            Console.WriteLine("Symetric Decrypted text: " + Encryption.SymetricDecrypt(EncryptedText));
        }

        //Test Asymetric encryption / decryption features
        private static void Test_Asymetric_Encryption_Class()
        {
            String TextToEncrypt = "PassPassPass";

            Console.WriteLine("Text to process: " + TextToEncrypt);

            String EncryptedText = Encryption.AsymetricEncrypt(TextToEncrypt);

            Console.WriteLine("Asymetric Encrypted text: " + EncryptedText);

            Console.WriteLine("Asymetric Decrypted text: " + Encryption.AsymetricDecrypt(EncryptedText));
        }

        private static void Test_Salt_and_Pass_Generator()
        {
            Encryption.ResetCryptoKeys();
        }

        private static void Test_Ordering_Table_List()
        {
            //Number of tables to process
            Int16 NumberOfTables = 10;

            #region Local Variables

            //This matrix will tell dependency count and dependency graph
            Int16[,] TableOrder = new Int16[NumberOfTables, NumberOfTables];
            Int16[] ReferenceCountArray = new Int16[NumberOfTables];
            Int16[] RowOrderArray = new Int16[NumberOfTables];

            String tablename = null;
            Int16 ReferenceCount;
            Boolean IncrementI = true;

            #endregion

            #region Generate Dependancy Table Graph and PreOrdering arrays

            //Load ReferenceCountArray
            //Load RowOrderArray
            //Load TableOrder

            //Matrix:

            //      t1  t2  t3  t4  t5
            //  t1  0   1   0   0   0
            //  t2  0   0   1   1   0
            //  t3  1   0   0   0   0
            //  t4  0   0   0   0   1
            //  t5  0   0   0   0   0
            //
            // Order: t5, t4, t1 (or t1, t4), t3, t2

            // Original version result: t5(ok), t4(ok), t3(bad), t1(bad), t2 (ok)

            /*
            ReferenceCountArray[0] = 1;
            ReferenceCountArray[1] = 2;
            ReferenceCountArray[2] = 1;
            ReferenceCountArray[3] = 1;
            ReferenceCountArray[4] = 0;
             */

            ReferenceCountArray[0] = 5;
            ReferenceCountArray[1] = 3;
            ReferenceCountArray[2] = 1;
            ReferenceCountArray[3] = 1;
            ReferenceCountArray[4] = 4;
            ReferenceCountArray[5] = 0;
            ReferenceCountArray[6] = 2;
            ReferenceCountArray[7] = 2;
            ReferenceCountArray[8] = 1;
            ReferenceCountArray[9] = 1;

            RowOrderArray[0] = 0;
            RowOrderArray[1] = 1;
            RowOrderArray[2] = 2;
            RowOrderArray[3] = 3;
            RowOrderArray[4] = 4;
            RowOrderArray[5] = 5;
            RowOrderArray[6] = 6;
            RowOrderArray[7] = 7;
            RowOrderArray[8] = 8;
            RowOrderArray[9] = 9;

            /* 
            TableOrder[0, 0] = 0;
            TableOrder[0, 1] = 1;
            TableOrder[0, 2] = 0;
            TableOrder[0, 3] = 0;
            TableOrder[0, 4] = 0;
            TableOrder[1, 0] = 0;
            TableOrder[1, 1] = 0;
            TableOrder[1, 2] = 1;
            TableOrder[1, 3] = 1;
            TableOrder[1, 4] = 0;
            TableOrder[2, 0] = 1;
            TableOrder[2, 1] = 0;
            TableOrder[2, 2] = 0;
            TableOrder[2, 3] = 0;
            TableOrder[2, 4] = 0;
            TableOrder[3, 0] = 0;
            TableOrder[3, 1] = 0;
            TableOrder[3, 2] = 0;
            TableOrder[3, 3] = 0;
            TableOrder[3, 4] = 1;
            TableOrder[4, 0] = 0;
            TableOrder[4, 1] = 0;
            TableOrder[4, 2] = 0;
            TableOrder[4, 3] = 0;
            TableOrder[4, 4] = 0;
            */

            TableOrder[0, 0] = 0; TableOrder[0, 1] = 1; TableOrder[0, 2] = 1; TableOrder[0, 3] = 1; TableOrder[0, 4] = 1; TableOrder[0, 5] = 1; TableOrder[0, 6] = 0; TableOrder[0, 7] = 0; TableOrder[0, 8] = 0; TableOrder[0, 9] = 0;
            TableOrder[1, 0] = 0; TableOrder[1, 1] = 0; TableOrder[1, 2] = 1; TableOrder[1, 3] = 1; TableOrder[1, 4] = 0; TableOrder[1, 5] = 0; TableOrder[1, 6] = 0; TableOrder[1, 7] = 0; TableOrder[1, 8] = 0; TableOrder[1, 9] = 1;
            TableOrder[2, 0] = 0; TableOrder[2, 1] = 0; TableOrder[2, 2] = 0; TableOrder[2, 3] = 0; TableOrder[2, 4] = 0; TableOrder[2, 5] = 0; TableOrder[2, 6] = 0; TableOrder[2, 7] = 0; TableOrder[2, 8] = 0; TableOrder[2, 9] = 1;
            TableOrder[3, 0] = 0; TableOrder[3, 1] = 0; TableOrder[3, 2] = 1; TableOrder[3, 3] = 0; TableOrder[3, 4] = 0; TableOrder[3, 5] = 0; TableOrder[3, 6] = 0; TableOrder[3, 7] = 0; TableOrder[3, 8] = 0; TableOrder[3, 9] = 0;
            TableOrder[4, 0] = 0; TableOrder[4, 1] = 1; TableOrder[4, 2] = 1; TableOrder[4, 3] = 1; TableOrder[4, 4] = 0; TableOrder[4, 5] = 0; TableOrder[4, 6] = 0; TableOrder[4, 7] = 0; TableOrder[4, 8] = 0; TableOrder[4, 9] = 0;
            TableOrder[5, 0] = 0; TableOrder[5, 1] = 0; TableOrder[5, 2] = 0; TableOrder[5, 3] = 0; TableOrder[5, 4] = 0; TableOrder[5, 5] = 0; TableOrder[5, 6] = 0; TableOrder[5, 7] = 0; TableOrder[5, 8] = 0; TableOrder[5, 9] = 0;
            TableOrder[6, 0] = 1; TableOrder[6, 1] = 1; TableOrder[6, 2] = 0; TableOrder[6, 3] = 0; TableOrder[6, 4] = 0; TableOrder[6, 5] = 0; TableOrder[6, 6] = 0; TableOrder[6, 7] = 0; TableOrder[6, 8] = 0; TableOrder[6, 9] = 0;
            TableOrder[7, 0] = 0; TableOrder[7, 1] = 0; TableOrder[7, 2] = 0; TableOrder[7, 3] = 0; TableOrder[7, 4] = 0; TableOrder[7, 5] = 0; TableOrder[7, 6] = 0; TableOrder[7, 7] = 0; TableOrder[7, 8] = 1; TableOrder[7, 9] = 1;
            TableOrder[8, 0] = 1; TableOrder[8, 1] = 0; TableOrder[8, 2] = 0; TableOrder[8, 3] = 0; TableOrder[8, 4] = 0; TableOrder[8, 5] = 0; TableOrder[8, 6] = 0; TableOrder[8, 7] = 0; TableOrder[8, 8] = 0; TableOrder[8, 9] = 0;
            TableOrder[9, 0] = 0; TableOrder[9, 1] = 0; TableOrder[9, 2] = 0; TableOrder[9, 3] = 0; TableOrder[9, 4] = 0; TableOrder[9, 5] = 1; TableOrder[9, 6] = 0; TableOrder[9, 7] = 0; TableOrder[9, 8] = 0; TableOrder[9, 9] = 0;

            
            List<String> lTables = new List<String>();

            lTables.Add("t1");
            lTables.Add("t2");
            lTables.Add("t3");
            lTables.Add("t4");
            lTables.Add("t5");
            lTables.Add("t6");
            lTables.Add("t7");
            lTables.Add("t8");
            lTables.Add("t9");
            lTables.Add("t10");

            List<String> OrderedList = new List<String>();

            #endregion

            #region Ordering Arrays and Solving conflicts

            Int16[] CorrectOrder = OrderGraph(TableOrder, (Int16)lTables.Count);

            //Map the correct order number to its table names.
            for (Int16 i = 0; i < CorrectOrder.Length; i++)
                OrderedList.Add(lTables[CorrectOrder[i]]);

            #endregion

            foreach (String s in OrderedList)
                Console.WriteLine(s);
        }

        /// <summary>
        /// Order a graph. Return results in an array (indirect index to right one).
        /// </summary>
        /// <param name="TableOrder">Dependancy matrix.</param>
        /// <param name="TableNumber">Number of tables.</param>
        /// <returns>Ordered array.</returns>
        private static Int16[] OrderGraph(Int16[,] TableOrder, Int16 TableNumber)
        {
            //This method will process the graph and order the tables
            //The ordering method here working is the "topological sort"

            //This graph is closed, all references are between tables to be ordered.            
            //Data in tables makes them to have no cycles in the graph.
            //If some tables have a circular-reference, data could not be inserted, so they will be empty. 
            //I won't take care of them because order of them doesn't matter (no data tables).

            Int16[] CorrectOrder = new Int16[TableNumber];
            List<Int16> NotProcessed = new List<Int16>();
            Int16 PredecessorCount, SuccessorCount;

            //This arrays will be the criteria to order
            Int16[,] LinkTable = new Int16[TableNumber, 2];

            Int16[] OrderedRank = new Int16[TableNumber];
            Int16[] PredecessorRank = new Int16[TableNumber];

            //Calculates predeccesor and succesor for each table
            for (Int16 i = 0; i < TableNumber; i++)
            {
                NotProcessed.Add(i);
                PredecessorCount = 0;
                SuccessorCount = 0;

                for (Int16 j = 0; j < TableNumber; j++)
                {
                    PredecessorCount += TableOrder[i, j];
                    SuccessorCount += TableOrder[j, i];
                }

                OrderedRank[i] = i;
                PredecessorRank[i] = PredecessorCount;

                LinkTable[i, 0] = PredecessorCount;
                LinkTable[i, 1] = SuccessorCount;
            }

            Array.Sort(PredecessorRank, OrderedRank);
            Int16 Index = 0, LastOrderedNode = 0;

            //Orders the graph
            while (NotProcessed.Count != 0)
            {
                //Find next node to process
                while (Index < TableNumber && !NotProcessed.Contains(OrderedRank[Index]))
                    Index++;

                //Recursive, long method, to order array
                ProcessNode(OrderedRank[Index], ref LastOrderedNode, ref TableOrder, ref LinkTable, ref TableNumber, ref NotProcessed, ref CorrectOrder);

                Index++;
            }

            return CorrectOrder;
        }

        /// <summary>
        /// Process the graph nodes, to list them in right order.
        /// </summary>
        /// <param name="Index">Node to analize.</param>
        /// <param name="LastOrderedNode">Last correctly ordered node.</param>
        /// <param name="TableOrder">Table Order matrix.</param>
        /// <param name="LinkTable">Precedessors and succesors count.</param>
        /// <param name="TableNumber">Number of tables.</param>
        /// <param name="NotProcessed">Unprocessed node list.</param>
        /// <param name="CorrectOrder">Array ordered to the moment of the method call.</param>
        private static void ProcessNode(Int16 Index, ref Int16 LastOrderedNode, ref Int16[,] TableOrder, ref Int16[,] LinkTable, ref Int16 TableNumber, ref List<Int16> NotProcessed, ref Int16[] CorrectOrder)
        {
            if (NotProcessed.Contains(Index) && NotProcessed.Count != 0)
            {
                Boolean OrderNode = false;

                if (LinkTable[Index, 0] == 0)
                    OrderNode = true;
                else
                {
                    //Let's assume order node is the right one.
                    OrderNode = true;

                    //Check its dependencies
                    for (Int16 i = 0; i < TableNumber; i++)
                        if (TableOrder[Index, i] == 1)
                            //If dependant node is not processed, abort checking
                            if (NotProcessed.Contains(i))
                            {
                                //Ops! A required predeccessor is not present in list...
                                OrderNode = false;
                                break;
                            }

                }

                //Process node and child nodes if node is to order
                if (OrderNode)
                {
                    CorrectOrder[LastOrderedNode] = Index;
                    LastOrderedNode++;
                    NotProcessed.Remove(Index);

                    //Process the childs recursively
                    for (Int16 i = 0; i < TableNumber; i++)
                        if (TableOrder[i, Index] == 1 && NotProcessed.Contains(i))
                            ProcessNode(i, ref LastOrderedNode, ref TableOrder, ref LinkTable, ref TableNumber, ref NotProcessed, ref CorrectOrder);
                }
            }
        }


    }
}
