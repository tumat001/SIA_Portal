using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using CommonDatabaseActionReusables.AccountManager;

namespace SIA_Portal.CustomAccessors.RequestableDocument
{
    public class RequestableDocument
    {

        internal RequestableDocument(int id, string documentName, byte[] noteDescription)
        {
            Id = id;
            DocumentName = documentName;
            NoteDescription = noteDescription;
        }


        public int Id { get; }

        public string DocumentName { get; }

        public byte[] NoteDescription { get; }

        public string GetNoteDescriptionAsString()
        {
            return StringUtilities.ConvertByteArrayToString(NoteDescription);
        }

        public Builder ConstructBuilderWithSameInfo()
        {
            var builder = new Builder();
            builder.DocumentName = DocumentName;
            builder.NoteDescription = NoteDescription;

            return builder;
        }

        //

        public override string ToString()
        {
            return DocumentName;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                return ((RequestableDocument)obj).Id == Id;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public class Builder
        {

            public Builder() { }

            public string DocumentName { set; get; }

            public byte[] NoteDescription { set; get; }


            public void SetNoteDescriptionWithString(string text)
            {
                NoteDescription = StringUtilities.ConvertStringToByteArray(text);
            }


            public RequestableDocument Build(int id)
            {
                return new RequestableDocument(id, DocumentName, NoteDescription);
            }

        }

    }
}