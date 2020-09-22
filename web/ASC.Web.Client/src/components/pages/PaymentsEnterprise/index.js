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
const { changeLanguage, changeDocumentTitle } = utils;
const { tablet, size } = Utils.device;

const StyledBody = styled.div`
  margin: 0 auto;
  max-width: 920px;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(4, min-content);
  @media ${tablet} {
    max-width: ${size.smallTablet}px;
  }
  @media (max-width: 632px) {
    min-width: ${size.mobile}px;
  }
`;

class Body extends React.Component {
  constructor(props) {
    super(props);
    const { t, organizationName } = this.props;

    changeDocumentTitle(`${t("Payments")} – ${organizationName}`);
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
    const { isLoaded } = this.props;

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
  const { organizationName } = auth.settings;
  const { isLoaded } = auth;
  const { licenseUpload } = payments;
  return {
    isLoaded,
    licenseUpload,
    organizationName,
  };
}
export default connect(mapStateToProps, {
  setPaymentsLicense,
  setCurrentProductId,
  getSettingsPayment,
  resetUploadedLicense,
  acceptPaymentsLicense,
})(withRouter(PaymentsEnterprise));
