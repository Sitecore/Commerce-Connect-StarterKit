//-----------------------------------------------------------------------
// <copyright file="AddressModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The AddressModel class.</summary>
//-----------------------------------------------------------------------
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
using System.ComponentModel.DataAnnotations;

namespace Sitecore.Commerce.StarterKit.Models
{
  public class AddressModel
  {
    public string PartyId { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    /// <value>
    /// The first name.
    /// </value>
    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    /// <value>
    /// The last name.
    /// </value>
    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the company.
    /// </summary>
    /// <value>
    /// The company.
    /// </value>
    [Display(Name = "Company")]
    public string Company { get; set; }

    /// <summary>
    /// Gets or sets the address1.
    /// </summary>
    /// <value>
    /// The address1.
    /// </value>
    [Required]
    [Display(Name = "Address Line 1")]
    public string Address1 { get; set; }

    /// <summary>
    /// Gets or sets the address2.
    /// </summary>
    /// <value>
    /// The address2.
    /// </value>
    [Display(Name = "Address Line 2")]
    public string Address2 { get; set; }

    /// <summary>
    /// Gets or sets the zip postal code.
    /// </summary>
    /// <value>
    /// The zip postal code.
    /// </value>
    [Required]
    [DataType(DataType.PostalCode)]
    [Display(Name = "Zip / postal code")]
    public string ZipPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    /// <value>
    /// The city.
    /// </value>
    [Required]
    [Display(Name = "City")]
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    /// <value>
    /// The country.
    /// </value>
    [Required]
    [Display(Name = "Country")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    /// <value>
    /// The phone number.
    /// </value>
    [Required(ErrorMessage = "Phone number is required")]
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Equals
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((AddressModel) obj);
    }

    /// <summary>
    /// Equals
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    protected bool Equals(AddressModel other)
    {
      return string.Equals(PartyId, other.PartyId) &&
             string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) &&
             string.Equals(Email, other.Email) && string.Equals(Company, other.Company) &&
             string.Equals(Address1, other.Address1) && string.Equals(Address2, other.Address2) &&
             string.Equals(ZipPostalCode, other.ZipPostalCode) && string.Equals(City, other.City) &&
             string.Equals(State, other.State) && string.Equals(Country, other.Country) &&
             string.Equals(PhoneNumber, other.PhoneNumber);
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (PartyId != null ? PartyId.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (Company != null ? Company.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (Address1 != null ? Address1.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (Address2 != null ? Address2.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (ZipPostalCode != null ? ZipPostalCode.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
        hashCode = (hashCode*397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
        return hashCode;
      }
    }
  }
}