import { makeAutoObservable } from "mobx";
import { defaultStore } from "../components/pages/Settings/categories/integration/SingleSignOn/sub-components/constants";

const regExps = {
  // source: https://regexr.com/3ok5o
  url: new RegExp(
    "([a-z]{1,2}tps?):\\/\\/((?:(?![/#?&]).)+)(\\/(?:(?:(?![#?&]).)+\\/)?)?((?:(?!\\.|$|\\?|#).)+)?(\\.(?:(?!\\?|$|#).)+)?(\\?(?:(?!$|#).)+)?(#.+)?"
  ),

  // source: https://regexr.com/2rhq7
  email: new RegExp(
    "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
  ),

  // source: https://regexr.com/38pvb
  phone: new RegExp(
    "^\\s*(?:\\+?(\\d{1,3}))?([-. (]*(\\d{3})[-. )]*)?((\\d{3})[-. ]*(\\d{2,4})(?:[-.x ]*(\\d+))?)\\s*$"
  ),
};

class SsoFormStore {
  uploadXmlUrl = "";

  enableSso = true;

  spLoginLabel = "";

  // idpSettings
  entityId = "";
  ssoUrl = "";
  ssoBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
  sloUrl = "";
  sloBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
  nameIdFormat = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";

  idp_certificate = "";
  idp_privateKey = null;
  idp_action = "signing";
  idp_certificates = [];

  // idpCertificateAdvanced
  idp_decryptAlgorithm = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
  // no checkbox for that
  ipd_decryptAssertions = false;
  idp_verifyAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
  idp_verifyAuthResponsesSign = false;
  idp_verifyLogoutRequestsSign = false;
  idp_verifyLogoutResponsesSign = false;

  sp_certificate = "";
  sp_privateKey = "";
  sp_action = "signing";
  sp_certificates = [];

  // spCertificateAdvanced
  // null for some reason and no checkbox
  sp_decryptAlgorithm = null;
  sp_encryptAlgorithm = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
  sp_encryptAssertions = false;
  sp_signAuthRequests = false;
  sp_signLogoutRequests = false;
  sp_signLogoutResponses = false;
  sp_signingAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
  // sp_verifyAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

  firstName = "";
  lastName = "";
  email = "";
  location = "";
  title = "";
  phone = "";

  hideAuthPage = false;

  // sp metadata
  sp_entityId = "";
  sp_assertionConsumerUrl = "";
  sp_singleLogoutUrl = "";

  // hide parts of form
  ServiceProviderSettings = true;
  ShowAdditionalParametersIdp = true;
  ShowAdditionalParametersSp = true;
  SPMetadata = true;
  isIdpModalVisible = false;
  isSpModalVisible = false;

  // errors
  uploadXmlUrlHasError = false;
  spLoginLabelHasError = false;

  entityIdHasError = false;
  ssoUrlHasError = false;
  sloUrlHasError = false;

  firstNameHasError = false;
  lastNameHasError = false;
  emailHasError = false;
  locationHasError = false;
  titleHasError = false;
  phoneHasError = false;

  sp_entityIdHasError = false;
  sp_assertionConsumerUrlHasError = false;
  sp_singleLogoutUrlHasError = false;

  // error messages
  uploadXmlUrlErrorMessage = null;
  spLoginLabelErrorMessage = null;

  entityIdErrorMessage = null;
  ssoUrlErrorMessage = null;
  sloUrlErrorMessage = null;

  firstNameErrorMessage = null;
  lastNameErrorMessage = null;
  emailErrorMessage = null;
  locationErrorMessage = null;
  titleErrorMessage = null;
  phoneErrorMessage = null;

  sp_entityIdErrorMessage = null;
  sp_assertionConsumerUrlErrorMessage = null;
  sp_singleLogoutUrlErrorMessage = null;

  constructor() {
    makeAutoObservable(this);
  }

  onSsoToggle = () => {
    this.enableSso = !this.enableSso;
  };

  onTextInputChange = (e) => {
    this[e.target.name] = e.target.value;
  };

  onBindingChange = (e) => {
    this[e.target.name] = e.target.value;
  };

  onComboBoxChange = (option, field) => {
    this[field] = option.key;
  };

  onHideClick = (e, label) => {
    this[label] = !this[label];
  };

  onCheckboxChange = (e) => {
    this[e.target.name] = e.target.checked;
  };

  onOpenIdpModal = () => {
    this.isIdpModalVisible = true;
  };

  onOpenSpModal = () => {
    this.isSpModalVisible = true;
  };

  onCloseModal = (e, modalVisible) => {
    this[modalVisible] = false;
  };

  onModalComboBoxChange = (option) => {
    this.spCertificateUsedFor = option.key;
  };

  onUploadXmlMetadata = async () => {
    const response = await fetch("./response.json");
    const metadataObject = await response.json();

    this.setFieldsFromObject(metadataObject);
  };

  setFieldsFromObject = (object) => {
    for (let key of Object.keys(object)) {
      if (typeof object[key] !== "object") {
        this[key] = object[key];

        this.setErrors(key, this[key]);
      } else {
        let prefix = "";

        if (key !== "fieldMapping" && key !== "idpSettings") {
          prefix = key.includes("idp") ? "idp_" : "sp_";
        }

        if (Array.isArray(object[key])) {
          this[`${prefix}Certificates`] = [...object[key]];
        } else {
          for (let field of Object.keys(object[key])) {
            this[`${prefix}${field}`] = object[key][field];

            this.setErrors(`${prefix}${field}`, this[`${prefix}${field}`]);
          }
        }
      }
    }
  };
  addCertificateToForm = (e, type) => {
    const action = this[`${type}_action`];
    const crt = this[`${type}_certificate`];
    const key = this[`${type}_privateKey`];

    try {
      const newCertificate = this.validateCertificate(action, crt, key);
      this[`${type}_certificates`] = [
        ...this[`${type}_certificates`],
        newCertificate,
      ];
    } catch (err) {
      console.log(err);
    }
  };

  validateCertificate = async (action, crt, key) => {
    const body = JSON.stringify({ action, crt, key });
    const params = {
      method: "POST",
      cors: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body,
    };

    const response = await fetch("http://jsonplaceholder.typicode.com/posts", {
      method: "POST",
      cors: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ some: "object" }),
    });

    const certificate = await response.json();

    return mockCertificate;
  };

  generateCertificate = async () => {
    const response = await fetch("./generatedCertificate.json");
    const certificateObject = await response.json();

    this.setGeneratedCertificate(certificateObject);
  };

  setGeneratedCertificate = (certificateObject) => {
    this.sp_certificate = certificateObject.crt;
    this.sp_privateKey = certificateObject.key;
  };

  resetForm = () => {
    for (let key of Object.keys(defaultStore)) {
      this[key] = defaultStore[key];
    }
  };

  onBlur = (e) => {
    const field = e.target.name;
    const value = e.target.value;

    this.setErrors(field, value);
  };

  setErrors = (field, value) => {
    if (typeof value === "boolean") return;

    const fieldError = `${field}HasError`;
    const fieldErrorMessage = `${field}ErrorMessage`;

    try {
      this.validate(value, this.getFieldType(field));
      this[fieldError] = false;
      this[fieldErrorMessage] = null;
    } catch (err) {
      this[fieldError] = true;
      this[fieldErrorMessage] = err.message;
    }
  };

  getFieldType = (field) => {
    if (field.toLowerCase().includes("url")) return "url";
    if (field.includes("entityId")) return "url";
    if (field.includes("email")) return "email";
    if (field.includes("phone")) return "phone";
    return "string";
  };

  validate = (string, type) => {
    string = string.trim();

    if (string.length === 0) throw new Error("EmptyFieldErrorMessage");

    if (type === "string") return true;

    if (regExps[type].test(string)) return true;
    else throw new Error(`${type}ErrorMessage`);
  };
}

const FormStore = new SsoFormStore();

export default FormStore;
