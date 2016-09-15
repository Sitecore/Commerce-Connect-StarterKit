// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentProperty.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the PaymentProperty class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file  
// except in compliance with the License. You may obtain a copy of the License at 
//       http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the  
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,  
// either express or implied. See the License for the specific language governing permissions  
// and limitations under the License. 
// --------------------------------------------------------------------- 

namespace Sitecore.Commerce.PaymentService
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a payment property.
    /// </summary>
    [DataContract]
    public class PaymentProperty : IEquatable<PaymentProperty>
    {
        private int displayHeight;
        private bool isHidden;
        private bool isPassword;
        private bool isReadOnly;
        private int sequenceNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProperty"/> class.
        /// </summary>
        public PaymentProperty()
        {
            this.displayHeight = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProperty"/> class.
        /// </summary>
        /// <param name="namespaceValue">The property namespace value.</param>
        /// <param name="nameValue">The property name value.</param>
        /// <param name="value">The property value.</param>
        public PaymentProperty(string namespaceValue, string nameValue, DateTime value)
        {
            this.displayHeight = 1;
            this.Namespace = namespaceValue;
            this.Name = nameValue;
            this.ValueType = DataType.DateTime;
            this.DateValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProperty"/> class.
        /// </summary>
        /// <param name="namespaceValue">The property namespace value.</param>
        /// <param name="nameValue">The property name value.</param>
        /// <param name="value">The property value.</param>
        public PaymentProperty(string namespaceValue, string nameValue, decimal value)
        {
            this.displayHeight = 1;
            this.Namespace = namespaceValue;
            this.Name = nameValue;
            this.ValueType = DataType.Decimal;
            this.DecimalValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProperty"/> class.
        /// </summary>
        /// <param name="namespaceValue">The property namespace value.</param>
        /// <param name="nameValue">The property name value.</param>
        /// <param name="value">The property value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "By Design.")]
        public PaymentProperty(string namespaceValue, string nameValue, string value)
        {
            this.displayHeight = 1;
            this.Namespace = namespaceValue;
            this.Name = nameValue;
            this.ValueType = DataType.String;
            this.StringValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProperty"/> class.
        /// </summary>
        /// <param name="namespaceValue">The property namespace value.</param>
        /// <param name="nameValue">The property name value.</param>
        /// <param name="propertyList">The property value.</param>
        public PaymentProperty(string namespaceValue, string nameValue, PaymentProperty[] propertyList)
        {
            this.displayHeight = 1;
            this.Namespace = namespaceValue;
            this.Name = nameValue;
            this.ValueType = DataType.PropertyList;
            this.PropertyList = propertyList;
        }

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        [DataMember]
        public DateTime DateValue { get; set; }

        /// <summary>
        /// Gets or sets the decimal value.
        /// </summary>
        [DataMember]
        public decimal DecimalValue { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlIgnore, DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display height.
        /// </summary>
        [DataMember]
        public int DisplayHeight
        {
            get
            {
                return this.displayHeight;
            }

            set
            {
                this.displayHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [XmlIgnore, DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is hidden.
        /// </summary>
        [DataMember]
        public bool IsHidden
        {
            get
            {
                return this.isHidden;
            }

            set
            {
                this.isHidden = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the property is a password.
        /// </summary>
        [DataMember]
        public bool IsPassword
        {
            get
            {
                return this.isPassword;
            }

            set
            {
                this.isPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the property is read-only.
        /// </summary>
        [DataMember]
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }

            set
            {
                this.isReadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the property namespace.
        /// </summary>
        [DataMember]
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the property list.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "By Design.")]
        [DataMember]
        public PaymentProperty[] PropertyList { get; set; }

        /// <summary>
        /// Gets or sets the property sequence number.
        /// </summary>
        [DataMember]
        public int SequenceNumber
        {
            get
            {
                return this.sequenceNumber;
            }

            set
            {
                this.sequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the stored string value.
        /// </summary>
        [DataMember]
        public string StoredStringValue
        {
            get
            {
                return this.StringValueField;
            }

            set
            {
                this.StringValueField = value;
            }
        }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        [XmlIgnore]
        public virtual string StringValue
        {
            get
            {
                return this.StringValueField;
            }

            set
            {
                this.StringValueField = value;
            }
        }

        /// <summary>
        /// Gets or sets the property value type.
        /// </summary>
        [DataMember]
        public DataType ValueType { get; set; }

        /// <summary>
        /// Gets or sets the string value field.
        /// </summary>
        protected string StringValueField { get; set; }

        /// <summary>
        /// Converts a hash to a property list.
        /// </summary>
        /// <param name="propertyHash">The property hash.</param>
        /// <returns>The property list.</returns>
        public static PaymentProperty[] ConvertHashToProperties(Dictionary<string, object> propertyHash)
        {
            List<PaymentProperty> list = new List<PaymentProperty>();
            if (propertyHash != null)
            {
                foreach (string str in propertyHash.Keys)
                {
                    foreach (string str2 in ((Dictionary<string, object>)propertyHash[str]).Keys)
                    {
                        PaymentProperty property = ((Dictionary<string, object>)propertyHash[str])[str2] as PaymentProperty;
                        if (property != null)
                        {
                            list.Add(property);
                        }
                        else
                        {
                            var hashtable = ((Dictionary<string, object>)propertyHash[str])[str2] as Dictionary<string, object>;
                            if (hashtable != null)
                            {
                                list.Add(new PaymentProperty(str, str2, ConvertHashToProperties(hashtable)));
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Converts a property array to XML.
        /// </summary>
        /// <param name="properties">The property array.</param>
        /// <returns>An XML string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "XML", Justification = "By design.")]
        public static string ConvertPropertyArrayToXML(object[] properties)
        {
            if ((properties == null) || (properties.Length == 0))
            {
                return string.Empty;
            }

            PaymentProperty[] propertyArray = ClearEncryption((PaymentProperty[])properties);
            XmlSerializer serializer = new XmlSerializer(typeof(PaymentProperty[]));
            StringBuilder builder = new StringBuilder("<![CDATA[");
            using (StringWriter writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                serializer.Serialize((TextWriter)writer, propertyArray);
                writer.Write("]]>");
                return builder.ToString();
            }
        }

        /// <summary>
        /// Converts a property array to a hashtable.
        /// </summary>
        /// <param name="properties">The property array.</param>
        /// <returns>A hashtable.</returns>
        public static Dictionary<string, object> ConvertToHashtable(PaymentProperty[] properties)
        {
            return InnerConvertToHashtable(properties, new Dictionary<string, object>());
        }

        /// <summary>
        /// Coverts XML to a property array.
        /// </summary>
        /// <param name="xml">The xml string.</param>
        /// <returns>A property array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "XML", Justification = "By Design.")]
        public static PaymentProperty[] ConvertXMLToPropertyArray(string xml)
        {
            PaymentProperty[] propertyArray;
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            if (xml.StartsWith("<![CDATA[", (StringComparison)StringComparison.OrdinalIgnoreCase))
            {
                xml = xml.Substring(9);
            }

            if (xml.EndsWith("]]>", (StringComparison)StringComparison.OrdinalIgnoreCase))
            {
                xml = xml.Substring(0, xml.Length - 3);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(PaymentProperty[]));
            using (StringReader reader = new StringReader(xml))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Prohibit;
                XmlReader reader2 = XmlReader.Create((TextReader)reader, settings);
                propertyArray = (PaymentProperty[])serializer.Deserialize(reader2);
            }
            
            return ClearEncryption(propertyArray);
        }

        /// <summary>
        /// Gets a property from a hashtable.
        /// </summary>
        /// <param name="hashtable">The hashtable.</param>
        /// <param name="requiredNamespace">The property namespace.</param>
        /// <param name="requiredPropertyName">The property name.</param>
        /// <returns>The property.</returns>
        public static PaymentProperty GetPropertyFromHashtable(Dictionary<string, object> hashtable, string requiredNamespace, string requiredPropertyName)
        {
            PaymentProperty property = null;
            if (hashtable != null)
            {
                Dictionary<string, object> hashtable2 = null;
                if (hashtable.ContainsKey(requiredNamespace))
                {
                    hashtable2 = hashtable[requiredNamespace] as Dictionary<string, object>;
                }

                if (hashtable2 == null)
                {
                    return property;
                }

                if (hashtable2.ContainsKey(requiredPropertyName))
                {
                    property = hashtable2[requiredPropertyName] as PaymentProperty;
                    if (property == null)
                    {
                        property = new PaymentProperty(requiredNamespace, requiredPropertyName, ConvertHashToProperties((Dictionary<string, object>)hashtable2[requiredPropertyName]));
                    }

                    return property;
                }

                if (hashtable2.ContainsKey("Properties"))
                {
                    Dictionary<string, object> hashtable3 = hashtable2["Properties"] as Dictionary<string, object>;
                    if (hashtable3 != null)
                    {
                        return GetPropertyFromHashtable(hashtable3, requiredNamespace, requiredPropertyName);
                    }
                }
            }

            return property;
        }

        /// <summary>
        /// Gets a property value from a hashtable.
        /// </summary>
        /// <param name="hashtable">The hashtable.</param>
        /// <param name="namespaceValue">The property namespace.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">Recieves the property value.</param>
        /// <returns>True if the property value was found, otherwise false.</returns>
        public static bool GetPropertyValue(Dictionary<string, object> hashtable, string namespaceValue, string name, out bool value)
        {
            bool flag = false;
            value = false;
            PaymentProperty property = GetPropertyFromHashtable(hashtable, namespaceValue, name);
            if ((property != null) && bool.TryParse(property.StringValue, out value))
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// Gets a property value from a hashtable.
        /// </summary>
        /// <param name="hashtable">The hashtable.</param>
        /// <param name="namespaceValue">The property namespace.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">Recieves the property value.</param>
        /// <returns>True if the property value was found, otherwise false.</returns>
        public static bool GetPropertyValue(Dictionary<string, object> hashtable, string namespaceValue, string name, out decimal value)
        {
            bool flag = false;
            value = new decimal();
            PaymentProperty property = GetPropertyFromHashtable(hashtable, namespaceValue, name);
            if (property != null)
            {
                flag = true;
                value = property.DecimalValue;
            }

            return flag;
        }

        /// <summary>
        /// Gets a property value from a hashtable.
        /// </summary>
        /// <param name="hashtable">The hashtable.</param>
        /// <param name="namespaceValue">The property namespace.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">Recieves the property value.</param>
        /// <returns>True if the property value was found, otherwise false.</returns>
        public static bool GetPropertyValue(Dictionary<string, object> hashtable, string namespaceValue, string name, out string value)
        {
            bool flag = false;
            value = string.Empty;
            PaymentProperty property = GetPropertyFromHashtable(hashtable, namespaceValue, name);
            if (property != null)
            {
                flag = true;
                value = property.StringValue;
            }

            return flag;
        }

        /// <summary>
        /// Gets a property value from a hashtable.
        /// </summary>
        /// <param name="hashtable">The hashtable.</param>
        /// <param name="namespaceValue">The property namespace.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">Recieves the property value.</param>
        /// <returns>True if the property value was found, otherwise false.</returns>
        public static bool GetPropertyValue(Dictionary<string, object> hashtable, string namespaceValue, string name, out PaymentProperty[] value)
        {
            bool flag = false;
            value = null;
            PaymentProperty property = GetPropertyFromHashtable(hashtable, namespaceValue, name);
            if (property != null)
            {
                flag = true;
                value = property.PropertyList;
            }

            return flag;
        }

        /// <summary>
        /// Gets a property value from a hashtable.
        /// </summary>
        /// <param name="hashtable">The hashtable.</param>
        /// <param name="namespaceValue">The property namespace.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">Recieves the property value.</param>
        /// <returns>True if the property value was found, otherwise false.</returns>
        public static bool GetPropertyValue(Dictionary<string, object> hashtable, string namespaceValue, string name, out DateTime value)
        {
            bool flag = false;
            value = default(DateTime);
            PaymentProperty property = GetPropertyFromHashtable(hashtable, namespaceValue, name);
            if (property != null)
            {
                flag = true;
                value = property.DateValue;
            }

            return flag;
        }

        /// <summary>
        /// Removes data encryption from a property array.
        /// </summary>
        /// <param name="properties">The property array.</param>
        /// <returns>The property array without data encryption.</returns>
        public static PaymentProperty[] RemoveDataEncryption(PaymentProperty[] properties)
        {
            List<PaymentProperty> list = null;
            if (properties != null)
            {
                list = new List<PaymentProperty>();
                foreach (PaymentProperty property in properties)
                {
                    switch (property.ValueType)
                    {
                        case DataType.String:
                            {
                                PaymentProperty property4 = new PaymentProperty(property.Namespace, property.Name, property.StringValue);
                                list.Add(property4);
                                break;
                            }

                        case DataType.Decimal:
                            {
                                PaymentProperty property3 = new PaymentProperty(property.Namespace, property.Name, property.DecimalValue);
                                list.Add(property3);
                                break;
                            }

                        case DataType.DateTime:
                            {
                                PaymentProperty property2 = new PaymentProperty(property.Namespace, property.Name, property.DateValue);
                                list.Add(property2);
                                break;
                            }

                        case DataType.PropertyList:
                            {
                                PaymentProperty[] propertyList = RemoveDataEncryption(property.PropertyList);
                                PaymentProperty property5 = new PaymentProperty(property.Namespace, property.Name, propertyList);
                                list.Add(property5);
                                break;
                            }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Compares this object to another object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(PaymentProperty other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.Namespace.Equals(other.Namespace, (StringComparison)StringComparison.OrdinalIgnoreCase) && this.Name.Equals(other.Name, (StringComparison)StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Sets metadata.
        /// </summary>
        /// <param name="displayNameValue">The display name.</param>
        /// <param name="descriptionValue">The description.</param>
        public void SetMetadata(string displayNameValue, string descriptionValue)
        {
            this.DisplayName = displayNameValue;
            this.Description = descriptionValue;
        }

        /// <summary>
        /// Sets metadata.
        /// </summary>
        /// <param name="displayNameValue">The display name.</param>
        /// <param name="descriptionValue">The description.</param>
        /// <param name="isPasswordValue">Specifies a password value.</param>
        /// <param name="isReadOnlyValue">Specifies a read-only value.</param>
        /// <param name="sequenceNumberValue">Specifies the sequence number.</param>
        public void SetMetadata(string displayNameValue, string descriptionValue, bool isPasswordValue, bool isReadOnlyValue, int sequenceNumberValue)
        {
            this.DisplayName = displayNameValue;
            this.Description = descriptionValue;
            this.isPassword = isPasswordValue;
            this.isReadOnly = isReadOnlyValue;
            this.sequenceNumber = sequenceNumberValue;
        }

        /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            switch (this.ValueType)
            {
                case DataType.String:
                    return this.StringValue;

                case DataType.Decimal:
                    return this.DecimalValue.ToString(CultureInfo.InvariantCulture);

                case DataType.DateTime:
                    return this.DateValue.ToString(CultureInfo.InvariantCulture);
            }

            return base.ToString();
        }

        internal static PaymentProperty[] ClearEncryption(PaymentProperty[] properties)
        {
            if (properties == null)
            {
                return null;
            }

            foreach (PaymentProperty property in properties)
            {
                if (property.ValueType == DataType.PropertyList)
                {
                    property.PropertyList = ClearEncryption(property.PropertyList);
                }
            }

            return properties;
        }

        private static Dictionary<string, object> InnerConvertToHashtable(PaymentProperty[] properties, Dictionary<string, object> hashTable)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            foreach (PaymentProperty property in properties)
            {
                Dictionary<string, object> hashtable = null;
                if (hashTable.ContainsKey(property.Namespace))
                {
                    hashtable = (Dictionary<string, object>)hashTable[property.Namespace];
                }
                else
                {
                    hashtable = new Dictionary<string, object>();
                    hashTable.Add(property.Namespace, hashtable);
                }

                if (hashtable.ContainsKey(property.Name))
                {
                    if (property.ValueType == DataType.PropertyList)
                    {
                        hashtable[property.Name] = InnerConvertToHashtable(property.PropertyList, new Dictionary<string, object>());
                    }
                    else
                    {
                        hashtable[property.Name] = property;
                    }
                }
                else if (property.ValueType == DataType.PropertyList)
                {
                    hashtable.Add(property.Name, InnerConvertToHashtable(property.PropertyList, new Dictionary<string, object>()));
                }
                else
                {
                    hashtable.Add(property.Name, property);
                }
            }

            return hashTable;
        }
    }
}
