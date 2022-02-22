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

  enableSso = false;

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

  // Field mapping
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
  ServiceProviderSettings = false;
  idp_showAdditionalParameters = true;
  sp_showAdditionalParameters = true;
  SPMetadata = false;
  idp_isModalVisible = false;
  sp_isModalVisible = false;

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
    this.idp_isModalVisible = true;
  };

  onOpenSpModal = () => {
    this.sp_isModalVisible = true;
  };

  onCloseModal = (e, modalVisible) => {
    this[modalVisible] = false;
  };

  onModalComboBoxChange = (option) => {
    this.spCertificateUsedFor = option.key;
  };

  onBlur = (e) => {
    const field = e.target.name;
    const value = e.target.value;

    this.setErrors(field, value);
  };

  resetForm = async () => {
    const params = {
      method: "DELETE",
      cors: "same-origin",
    };

    try {
      const response = await fetch("/somewhere", params);
      if (response.ok) {
        this.resetField();
      } else throw new Error("error");
    } catch (err) {
      console.log(err);
    }
  };

  resetField = () => {
    for (let key of Object.keys(defaultStore)) {
      this[key] = defaultStore[key];
    }
  };

  onEditClick = (e, certificate, prefix) => {
    this[`${prefix}_certificate`] = certificate.crt;
    this[`${prefix}_privateKey`] = certificate.key;
    this[`${prefix}_action`] = certificate.action;
    this[`${prefix}_isModalVisible`] = true;
  };

  onDeleteClick = (e, crtToDelete, prefix) => {
    this[`${prefix}_certificates`] = this[`${prefix}_certificates`].filter(
      (certificate) => certificate.crt !== crtToDelete
    );
  };

  onLoadXmlMetadata = async () => {
    const body = JSON.stringify({ url: this.uploadXmlUrl });
    const params = {
      method: "POST",
      cors: "same-origin",
      headers: {
        "Content-Type": "application/json;charset=utf-8",
      },
      body: body,
    };

    try {
      const response = await fetch("/somewhere", params);
      if (response.ok) {
        const metadataObject = await response.json();
        this.setFieldsFromObject(metadataObject);
      } else throw new Error("error");
    } catch (err) {
      console.log(err);
    }
  };

  onUploadXmlMetadata = async (file) => {
    if (!file.type.includes("text/xml")) return console.log("invalid format");

    const data = new FormData();
    data.append("file", file);

    const params = {
      method: "POST",
      cors: "same-origin",
      body: data,
    };

    try {
      const response = await fetch("/somewhere", params);
      if (response.ok) {
        const metadataObject = await response.json();
        this.setFieldsFromObject(metadataObject);
      } else throw new Error("error");
    } catch (err) {
      console.log(err);
    }
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
          this[`${prefix}certificates`] = object[key].slice();
        } else {
          for (let field of Object.keys(object[key])) {
            this[`${prefix}${field}`] = object[key][field];

            this.setErrors(`${prefix}${field}`, this[`${prefix}${field}`]);
          }
        }
      }
    }
  };
  addCertificateToForm = (e, prefix) => {
    const action = this[`${prefix}_action`];
    const crt = this[`${prefix}_certificate`];
    const key = this[`${prefix}_privateKey`];

    try {
      const newCertificate = this.validateCertificate(action, crt, key);
      this[`${prefix}_certificates`] = [
        ...this[`${prefix}_certificates`],
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

    try {
      const response = await fetch("/somewhere", params);
      if (response.ok) {
        return await response.json();
      } else throw new Error("error");
    } catch (err) {
      console.log(err);
    }
  };

  generateCertificate = async () => {
    try {
      const response = await fetch("/somewhere");
      if (response.ok) {
        const certificateObject = await response.json();
        this.setGeneratedCertificate(certificateObject);
      } else throw new Error("error");
    } catch (err) {
      console.log(err);
    }
  };

  setGeneratedCertificate = (certificateObject) => {
    this.sp_certificate = certificateObject.crt;
    this.sp_privateKey = certificateObject.key;
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

  onSubmit = async () => {
    const body = JSON.stringify({
      enableSso: true,
      spLoginLabel: this.spLoginLabel,
      idpSettings: {
        entityId: this.entityId,
        ssoUrl: this.ssoUrl,
        ssoBinding: this.ssoBinding,
        sloUrl: this.sloUrl,
        sloBinding: this.sloBinding,
        nameIdFormat: this.nameIdFormat,
      },
      idpCertificates: this.idp_certificates,
      idpCertificateAdvanced: {
        verifyAlgorithm: this.idp_verifyAlgorithm,
        verifyAuthResponsesSign: this.idp_verifyAuthResponsesSign,
        verifyLogoutRequestsSign: this.idp_verifyLogoutRequestsSign,
        verifyLogoutResponsesSign: this.idp_verifyLogoutResponsesSign,
        decryptAlgorithm: this.idp_decryptAlgorithm,
        // ?
        decryptAssertions: false,
      },
      spCertificates: this.sp_certificates,
      spCertificateAdvanced: {
        decryptAlgorithm: this.sp_decryptAlgorithm,
        signingAlgorithm: this.sp_signingAlgorithm,
        signAuthRequests: this.sp_signAuthRequests,
        signLogoutRequests: this.sp_signLogoutRequests,
        signLogoutResponses: this.sp_signLogoutResponses,
        encryptAlgorithm: this.sp_encryptAlgorithm,
        encryptAssertions: this.sp_encryptAssertions,
      },
      fieldMapping: {
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        title: this.title,
        location: this.location,
        phone: this.phone,
      },
      hideAuthPage: this.hideAuthPage,
    });

    const params = {
      method: "POST",
      cors: "same-origin",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded;charset=utf-8",
      },
      body,
    };

    try {
      const response = await fetch("./somewhere");
      if (!response.ok) throw new Error("Some error");
      console.log("success");
    } catch (err) {
      console.log(err);
    }
  };
}

const FormStore = new SsoFormStore();

export default FormStore;
