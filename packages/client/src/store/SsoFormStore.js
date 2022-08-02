import { makeAutoObservable } from "mobx";
import {
  generateCerts,
  getCurrentSsoSettings,
  loadXmlMetadata,
  resetSsoForm,
  submitSsoForm,
  uploadXmlMetadata,
  validateCerts,
} from "@docspace/common/api/settings";
import toastr from "@docspace/components/toast/toastr";
import { BINDING_POST, BINDING_REDIRECT } from "../helpers/constants";
import isEqual from "lodash/isEqual";

class SsoFormStore {
  isSsoEnabled = false;

  set isSsoEnabled(value) {
    this.isSsoEnabled = value;
  }

  enableSso = false;

  uploadXmlUrl = "";

  spLoginLabel = "";

  isLoadingXml = false;

  // idpSettings
  entityId = "";
  ssoUrlPost = "";
  ssoUrlRedirect = "";
  ssoBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
  sloUrlPost = "";
  sloUrlRedirect = "";
  sloBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
  nameIdFormat = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";

  idpCertificate = "";
  idpPrivateKey = null;
  idpAction = "signing";
  idpCertificates = [];

  // idpCertificateAdvanced
  idpDecryptAlgorithm = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
  // no checkbox for that
  ipdDecryptAssertions = false;
  idpVerifyAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
  idpVerifyAuthResponsesSign = false;
  idpVerifyLogoutRequestsSign = false;
  idpVerifyLogoutResponsesSign = false;

  spCertificate = "";
  spPrivateKey = "";
  spAction = "signing";
  spCertificates = [];

  // spCertificateAdvanced
  // null for some reason and no checkbox
  spDecryptAlgorithm = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
  spEncryptAlgorithm = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
  spEncryptAssertions = false;
  spSignAuthRequests = false;
  spSignLogoutRequests = false;
  spSignLogoutResponses = false;
  spSigningAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
  // spVerifyAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

  // Field mapping
  firstName = "";
  lastName = "";
  email = "";
  location = "";
  title = "";
  phone = "";

  hideAuthPage = false;

  // sp metadata
  spEntityId = "";
  spAssertionConsumerUrl = "";
  spSingleLogoutUrl = "";

  // hide parts of form
  serviceProviderSettings = false;
  idpShowAdditionalParameters = true;
  spShowAdditionalParameters = true;
  spMetadata = false;
  idpIsModalVisible = false;
  spIsModalVisible = false;
  confirmationDisableModal = false;
  confirmationResetModal = false;

  // errors
  uploadXmlUrlHasError = false;
  spLoginLabelHasError = false;

  entityIdHasError = false;
  ssoUrlPostHasError = false;
  ssoUrlRedirectHasError = false;
  sloUrlPostHasError = false;
  sloUrlRedirectHasError = false;

  firstNameHasError = false;
  lastNameHasError = false;
  emailHasError = false;
  locationHasError = false;
  titleHasError = false;
  phoneHasError = false;

  // error messages
  //uploadXmlUrlErrorMessage = null;

  errorMessage = null;

  isSubmitLoading = false;
  isGeneratedCertificate = false;

  defaultSettings = null;

  constructor() {
    makeAutoObservable(this);
  }

  load = async () => {
    try {
      const res = await getCurrentSsoSettings();
      this.isSsoEnabled = res.enableSso;
      this.spMetadata = res.enableSso;
      this.defaultSettings = res;
      this.setFieldsFromObject(res);
    } catch (err) {
      console.log(err);
    }
  };

  ssoToggle = () => {
    if (!this.enableSso) {
      this.enableSso = true;
      this.serviceProviderSettings = true;
    } else {
      this.enableSso = false;
    }

    for (let key in this) {
      if (key.includes("ErrorMessage")) this[key] = null;
    }
  };

  setInput = (e) => {
    this[e.target.name] = e.target.value;
  };

  setComboBox = (option, field) => {
    this[field] = option.key;
  };

  setHideLabel = (label) => {
    this[label] = !this[label];
  };

  setCheckbox = (e) => {
    this[e.target.name] = e.target.checked;
  };

  openIdpModal = () => {
    this.idpIsModalVisible = true;
  };

  openSpModal = () => {
    this.spIsModalVisible = true;
  };

  closeIdpModal = () => {
    this.idpIsModalVisible = false;
  };

  closeSpModal = () => {
    this.spIsModalVisible = false;
  };

  setComboBoxOption = (option) => {
    this.spCertificateUsedFor = option.key;
  };

  disableSso = () => {
    this.isSsoEnabled = false;
  };

  openConfirmationDisableModal = () => {
    this.confirmationDisableModal = true;
  };

  closeConfirmationDisableModal = () => {
    this.confirmationDisableModal = false;
  };

  openResetModal = () => {
    this.confirmationResetModal = true;
  };

  closeResetModal = () => {
    this.confirmationResetModal = false;
  };

  confirmDisable = () => {
    this.disableSso();
    this.ssoToggle();
    this.confirmationDisableModal = false;
  };

  confirmReset = () => {
    this.resetForm();
    this.disableSso();
    this.serviceProviderSettings = false;
    this.spMetadata = false;
    this.confirmationResetModal = false;
  };

  uploadByUrl = async () => {
    const data = { url: this.uploadXmlUrl };

    try {
      this.isLoadingXml = true;
      const response = await loadXmlMetadata(data);
      this.setFieldsFromMetaData(response.data.meta);
      this.isLoadingXml = false;
    } catch (err) {
      this.isLoadingXml = false;
      toastr.error(err);
      console.error(err);
    }
  };

  uploadXml = async (file) => {
    if (!file.type.includes("text/xml")) return console.log("invalid format");

    const data = new FormData();
    data.append("metadata", file);

    try {
      this.isLoadingXml = true;
      const response = await uploadXmlMetadata(data);
      this.setFieldsFromMetaData(response.data.meta);
      this.isLoadingXml = false;
    } catch (err) {
      this.isLoadingXml = false;
      toastr.error(err);
      console.error(err);
    }
  };

  validateCertificate = async (crts) => {
    const data = { certs: crts };

    try {
      return await validateCerts(data);
    } catch (err) {
      toastr.error(err);
      console.error(err);
    }
  };

  generateCertificate = async () => {
    try {
      this.isGeneratedCertificate = true;

      const res = await generateCerts();
      this.setGeneratedCertificate(res.data);

      this.isGeneratedCertificate = false;
    } catch (err) {
      this.isGeneratedCertificate = false;
      toastr.error(err);
      console.error(err);
    }
  };

  getSettings = () => {
    const ssoUrl =
      this.ssoBinding === "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"
        ? this.ssoUrlPost
        : this.ssoUrlRedirect;
    const sloUrl =
      this.sloBinding === "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"
        ? this.sloUrlPost
        : this.sloUrlRedirect;

    return {
      enableSso: this.enableSso,
      spLoginLabel: this.spLoginLabel,
      idpSettings: {
        entityId: this.entityId,
        ssoUrl: ssoUrl,
        ssoBinding: this.ssoBinding,
        sloUrl: sloUrl,
        sloBinding: this.sloBinding,
        nameIdFormat: this.nameIdFormat,
      },
      idpCertificates: this.idpCertificates,
      idpCertificateAdvanced: {
        verifyAlgorithm: this.idpVerifyAlgorithm,
        verifyAuthResponsesSign: this.idpVerifyAuthResponsesSign,
        verifyLogoutRequestsSign: this.idpVerifyLogoutRequestsSign,
        verifyLogoutResponsesSign: this.idpVerifyLogoutResponsesSign,
        decryptAlgorithm: this.idpDecryptAlgorithm,
        decryptAssertions: false,
      },
      spCertificates: this.spCertificates,
      spCertificateAdvanced: {
        decryptAlgorithm: this.spDecryptAlgorithm,
        signingAlgorithm: this.spSigningAlgorithm,
        signAuthRequests: this.spSignAuthRequests,
        signLogoutRequests: this.spSignLogoutRequests,
        signLogoutResponses: this.spSignLogoutResponses,
        encryptAlgorithm: this.spEncryptAlgorithm,
        encryptAssertions: this.spEncryptAssertions,
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
    };
  };
  saveSsoSettings = async (t) => {
    const settings = this.getSettings();
    const data = { serializeSettings: JSON.stringify(settings) };

    this.isSubmitLoading = true;

    try {
      await submitSsoForm(data);
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
      this.isSubmitLoading = false;
      this.spMetadata = true;
      this.defaultSettings = settings;
    } catch (err) {
      toastr.error(err);
      console.error(err);
      this.isSubmitLoading = false;
    }
  };

  resetForm = async () => {
    try {
      const response = await resetSsoForm();

      this.setFieldsFromObject(response);
    } catch (err) {
      toastr.error(err);
      console.error(err);
    }
  };

  setFieldsFromObject = (object) => {
    for (let key of Object.keys(object)) {
      if (typeof object[key] !== "object") {
        this[key] = object[key];
      } else {
        let prefix = "";

        if (key === "idpSettings") {
          this.setSsoUrls(object[key]);
          this.setSloUrls(object[key]);
        }

        if (key !== "fieldMapping" && key !== "idpSettings") {
          prefix = key.includes("idp") ? "idp" : "sp";
        }

        if (Array.isArray(object[key])) {
          this[`${prefix}certificates`] = object[key].slice();
        } else {
          for (let field of Object.keys(object[key])) {
            this[`${prefix}${field}`] = object[key][field];
          }
        }
      }
    }
  };

  setSsoUrls = (o) => {
    switch (o.ssoBinding) {
      case BINDING_POST:
        this.ssoUrlPost = o.ssoUrl;
        break;
      case BINDING_REDIRECT:
        this.ssoUrlRedirect = o.ssoUrl;
    }
  };

  setSloUrls = (o) => {
    switch (o.sloBinding) {
      case BINDING_POST:
        this.sloUrlPost = o.ssoUrl;
        break;
      case BINDING_REDIRECT:
        this.sloUrlRedirect = o.ssoUrl;
    }
  };

  setFieldsFromMetaData = async (meta) => {
    if (meta.entityID) {
      this.entityId = meta.entityID;
    }

    if (meta.singleSignOnService) {
      this.ssoUrlPost =
        meta.singleSignOnService[
          "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"
        ];
      this.ssoUrlRedirect =
        meta.singleSignOnService[
          "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"
        ];
    }

    if (meta.singleLogoutService) {
      this.sloBinding = meta.singleLogoutService.binding;
      if (
        meta.singleLogoutService.binding ===
        "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"
      ) {
        this.sloUrlRedirect = meta.singleLogoutService.location;
      }

      if (
        meta.singleLogoutService.binding ===
        "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"
      ) {
        this.sloUrlPost = meta.singleLogoutService.location;
      }
    }

    if (meta.nameIDFormat) {
      this.nameIdFormat = meta.nameIDFormat;
    }

    if (meta.certificate) {
      let data = [];

      if (meta.certificate.signing) {
        if (Array.isArray(meta.certificate.signing)) {
          meta.certificate.signing = this.getUniqueItems(
            meta.certificate.signing
          ).reverse();
          meta.certificate.signing.forEach((signingCrt) => {
            data.push({
              crt: signingCrt.trim(),
              key: null,
              action: "verification",
            });
          });
        } else {
          data.push({
            crt: meta.certificate.signing.trim(),
            key: null,
            action: "verification",
          });
        }
      }

      const newCertificates = await this.validateCertificate(data);

      newCertificates.data.map((cert) => {
        if (newCertificates.data.length > 1) {
          this.idpCertificates = [...this.idpCertificates, cert];
        } else {
          this.idpCertificates = [cert];
        }

        if (cert.action === "verification") {
          this.idpVerifyAuthResponsesSign = true;
          this.idpVerifyLogoutRequestsSign = true;
        }
        if (cert.action === "decrypt") {
          this.idpVerifyLogoutResponsesSign = true;
        }
        if (cert.action === "verification and decrypt") {
          this.idpVerifyAuthResponsesSign = true;
          this.idpVerifyLogoutRequestsSign = true;
          this.idpVerifyLogoutResponsesSign = true;
        }
      });
    }
  };

  getUniqueItems = (array) => {
    return array.filter((item, index, array) => array.indexOf(item) == index);
  };

  setSpCertificate = (certificate) => {
    this.spCertificate = certificate.crt;
    this.spPrivateKey = certificate.key;
    this.spAction = certificate.action;
    this.spIsModalVisible = true;
  };

  setIdpCertificate = (certificate) => {
    this.idpCertificate = certificate.crt;
    this.idpPrivateKey = certificate.key;
    this.idpAction = certificate.action;
    this.idpIsModalVisible = true;
  };

  delSpCertificate = (cert) => {
    this.spCertificates = this.spCertificates.filter(
      (certificate) => certificate.crt !== cert
    );
  };

  delIdpCertificate = (cert) => {
    this.idpCertificates = this.idpCertificates.filter(
      (certificate) => certificate.crt !== cert
    );
  };

  addSpCertificate = async () => {
    const data = [
      {
        crt: this.spCertificate,
        key: this.spPrivateKey,
        action: this.spAction,
      },
    ];

    try {
      const res = await this.validateCertificate(data);
      const newCertificates = res.data;
      newCertificates.map((cert) => {
        this.spCertificates = [...this.spCertificates, cert];
      });
      this.closeSpModal();
    } catch (err) {
      toastr.error(err);
      console.error(err);
    }
  };

  addIdpCertificate = async () => {
    const data = [
      {
        crt: this.idpCertificate,
        key: this.idpPrivateKey,
        action: this.idpAction,
      },
    ];

    try {
      const res = await this.validateCertificate(data);
      const newCertificates = res.data;
      newCertificates.map((cert) => {
        this.idpCertificates = [...this.idpCertificates, cert];
      });
      this.closeIdpModal();
    } catch (err) {
      toastr.error(err);
      console.error(err);
    }
  };

  setGeneratedCertificate = (certificateObject) => {
    this.spCertificate = certificateObject.crt;
    this.spPrivateKey = certificateObject.key;
  };

  getError = (field) => {
    const fieldError = `${field}HasError`;
    console.log("getError", fieldError);
    return this[fieldError] !== null ? true : false;
  };

  setError = (field, value) => {
    if (typeof value === "boolean") return;

    const fieldError = `${field}HasError`;

    try {
      this.validate(value);
      this[fieldError] = false;
      this.errorMessage = null;
    } catch (err) {
      this[fieldError] = true;
      this.errorMessage = err.message;
    }
  };

  hideError = (field) => {
    const fieldError = `${field}HasError`;
    this[fieldError] = false;
    this.errorMessage = null;
  };

  validate = (string) => {
    if (string.trim().length === 0) throw new Error("EmptyFieldErrorMessage");
    else return true;
  };

  downloadMetadata = async () => {
    window.open("/sso/metadata", "_blank");
  };

  get hasErrors() {
    for (let key in this) {
      if (key.includes("ErrorMessage") && this[key] !== null) return true;
    }
    return false;
  }

  get hasChanges() {
    const currentSettings = this.getSettings();
    return !isEqual(currentSettings, this.defaultSettings);
  }
}

export default SsoFormStore;
