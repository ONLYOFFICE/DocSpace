import React, { useEffect } from "react";
import { PageLayout, utils, store } from "asc-web-common";
import { Loader, utils as Utils, toastr } from "asc-web-components";
import styled from "styled-components";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import HeaderContainer from "./sub-components/headerContainer";
import AdvantagesContainer from "./sub-components/advantagesContainer";
import ButtonContainer from "./sub-components/buttonContainer";
import ContactContainer from "./sub-components/contactContainer";
import {
  setPaymentsLicense,
  getSettingsPayment,
  resetUploadedLicense,
  acceptPaymentsLicense,
} from "../../../store/payments/actions";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});
const { setCurrentProductId } = store.auth.actions;
const { changeLanguage } = utils;
const { tablet } = Utils.device;

const StyledBody = styled.div`
  margin: 0 auto;
  max-width: 920px;

  @media ${tablet} {
    max-width: 600px;
    margin: 0 auto;
  }
  @media (max-width: 632px) {
    /* max-width: 343px; */
    margin: 0 auto;
  }
`;

class Body extends React.PureComponent {
  constructor(props) {
    super(props);
    const { t } = this.props;

    document.title = `${t("Payments")} – ${t("OrganizationName")}`; //убрать орг нейм
  }

  componentDidMount() {
    const {
      getSettingsPayment,
      currentProductId,
      setCurrentProductId,
    } = this.props;

    currentProductId !== "payments" && setCurrentProductId("payments");
    getSettingsPayment();
  }

  componentDidUpdate() {
    const {
      licenseUpload,
      resetUploadedLicense,
      acceptPaymentsLicense,
    } = this.props;

    if (licenseUpload) {
      acceptPaymentsLicense();
      resetUploadedLicense();
    }
  }

  onClickUpload = (file) => {
    const { setPaymentsLicense, t } = this.props;

    let fd = new FormData();
    fd.append("files", file);

    setPaymentsLicense(null, fd)
      .then(() => {
        toastr.success(t("LoadingLicenseSuccess"), "", 0, true);
      })
      .catch((error) => {
        toastr.error(t("LoadingLicenseError"), t("LicenseIsNotValid"), 0, true);
        console.log(error);
      });
  };

  onClickBuy = (e) => {
    window.open(e.target.value, "_blank");
  };

  render() {
    const { isLoaded, t } = this.props;

    return !isLoaded ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBody>
        <HeaderContainer />
        <AdvantagesContainer />
        <ButtonContainer
          onClickBuy={this.onClickBuy}
          onClickUpload={this.onClickUpload}
        />
        <ContactContainer />
      </StyledBody>
    );
  }
}
const PaymentsWrapper = withTranslation()(Body);
const PaymentsEnterprise = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <PageLayout>
      <PageLayout.SectionBody>
        <PaymentsWrapper {...props} i18n={i18n} />
      </PageLayout.SectionBody>
    </PageLayout>
  );
};

PaymentsEnterprise.propTypes = {
  isLoaded: PropTypes.bool,
};

function mapStateToProps({ auth, payments }) {
  return {
    isLoaded: auth.isLoaded,
    licenseUpload: payments.licenseUpload,
  };
}
export default connect(mapStateToProps, {
  setPaymentsLicense,
  setCurrentProductId,
  getSettingsPayment,
  resetUploadedLicense,
  acceptPaymentsLicense,
})(withRouter(PaymentsEnterprise));
