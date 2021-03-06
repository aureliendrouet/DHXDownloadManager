﻿﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
namespace DHXDownloadManager.Tests
{
    /// <summary>
    /// Checks a downloaded memory stream against an image the test
    /// ships with.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MemoryStreamTest<T> : Test<T> where T : IDownloadEngine, new()
    {
        int succeed = -1;

        public MemoryStreamTest(Tests<T> parent) : base(parent) { }

        override protected IEnumerator _DoTest()
        {
            yield return base._DoTest();

            Start();

            Manifest metadata = new ManifestMemoryStream("https://s3.amazonaws.com/piko_public/Test.png", ManifestFileStream.Flags.None);
            ((ManifestStream)(metadata)).OnAssetStreamFailed += FileStreamTest_OnAssetStreamFailed;
            ((ManifestStream)(metadata)).OnAssetStreamSuccess += FileStreamTest_OnAssetStreamSuccess;
            _Parent._Manager.AddDownload(metadata);

            while (succeed == -1)
            {
                yield return null;
            }
            Finish();
            if (succeed == 0)
                Success();
            else
                Fail();

            yield return null;
        }

        void FileStreamTest_OnAssetStreamSuccess(Manifest metaData, System.IO.Stream stream)
        {
            System.IO.MemoryStream ms = (System.IO.MemoryStream)stream;
            succeed = 0;
            try
            {
                string file1path = Application.streamingAssetsPath + "/DownloadManager/Tests/Test.png";
                byte[] bytes1 = System.IO.File.ReadAllBytes(file1path);

                byte[] bytes2 = ms.GetBuffer();

                bool areEqual = bytes2.SequenceEqual(bytes1);

                if (areEqual)
                    succeed = 0;
                else
                    succeed = 1;
            }
            catch
            {
                succeed = 1;
            }
            finally
            { }
        }

        void FileStreamTest_OnAssetStreamFailed(Manifest metaData, System.IO.Stream stream)
        {
            succeed = 1;
        }
    }
}