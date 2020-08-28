import React from "react";
import { PageLayout, utils } from "asc-web-common";
import { Loader, utils as Utils } from "asc-web-components";
import styled from "styled-components";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import HeaderContainer from "./sub-components/header-container";
import AdvantagesContainer from "./sub-components/advantages-container";
import ButtonContainer from "./sub-components/button-container";
import ContactContainer from "./sub-components/contact-container";
import ModalDialogContainer from "./sub-components/modal-dialog-container";
import { setLicense } from "../../../store/payments/actions";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});

const { changeLanguage } = utils;
const { tablet, mobile } = Utils.device;

const StyledBody = styled.div`
  margin: 0 auto;
  width: 920px;

  @media ${tablet} {
    width: 600px;
    margin: 0 auto;
  }
  @media ${mobile} {
    width: 343px;
    margin: 0 auto;
  }
`;

class Body extends React.PureComponent {
  constructor(props) {
    super(props);
    const { t } = this.props;
    this.state = {
      errorMessage: null,
      license: false,
      hasErrorLicense: false,
      visibleModalDialog: false,
    };

    document.title = `${t("Payments")}`;
  }

  onButtonClickUpload = (file) => {
    const { setLicense } = this.props;
    let fd = new FormData();
    fd.append("files", file);

    setLicense(null, fd).catch((e) =>
      this.setState({
        errorMessage: e,
        hasErrorLicense: true,
        visibleModalDialog: true,
      })
    );
  };
  onButtonClickBuy = (e) => {
    window.open(e.target.value, "_blank");
  };
  onCloseModalDialog = () => {
    this.setState({
      visibleModalDialog: false,
      errorMessage: null,
    });
  };
  render() {
    const {
      isLoaded,
      salesEmail,
      helpUrl,
      buyUrl,
      dateExpires,
      t,
      createPortals,
    } = this.props;

    const { hasErrorLicense, errorMessage, visibleModalDialog } = this.state;
    return !isLoaded ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBody>
        <HeaderContainer
          t={t}
          dateExpires={dateExpires}
          createPortals={createPortals}
        />
        <AdvantagesContainer t={t} />
        <ModalDialogContainer
          t={t}
          visibleModalDialog={visibleModalDialog}
          errorMessage={errorMessage}
          onCloseModalDialog={this.onCloseModalDialog}
        />
        <ButtonContainer
          t={t}
          buyUrl={buyUrl}
          hasErrorLicense={hasErrorLicense}
          onButtonClickBuy={this.onButtonClickBuy}
          onButtonClickUpload={this.onButtonClickUpload}
        />
        <ContactContainer t={t} salesEmail={salesEmail} helpUrl={helpUrl} />
      </StyledBody>
    );
  }
}
const PaymentsWrapper = withTranslation()(Body);
const PaymentsEnterprise = (props) => {
  changeLanguage(i18n);
  return (
    <PageLayout>
      <PageLayout.SectionBody>
        <PaymentsWrapper {...props} i18n={i18n} />
      </PageLayout.SectionBody>
    </PageLayout>
  );
};

PaymentsEnterprise.propTypes = {
  standAloneMode: PropTypes.bool,
  isLoaded: PropTypes.bool,
};

function mapStateToProps(state) {
  return {
    isLoaded: state.auth.isLoaded,
    salesEmail: state.payments.salesEmail,
    helpUrl: state.payments.helpUrl,
    buyUrl: state.payments.buyUrl,
    dateExpires: state.payments.dateExpires,
    createPortals: state.payments.createPortals,
  };
}
export default connect(mapStateToProps, {
  setLicense,
})(withRouter(PaymentsEnterprise));
