import React from "react";
import { PageLayout, utils, store } from "asc-web-common";
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
import {
  setLicense /*getPortalCultures*/,
} from "../../../store/payments/actions";
import { createI18N } from "../../../helpers/i18n";
import moment from "moment";
const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});
const { setCurrentProductId } = store.auth.actions;
const { changeLanguage } = utils;
const { tablet } = Utils.device;

const StyledBody = styled.div`
  margin: 0 auto;
  width: 920px;

  @media ${tablet} {
    width: 600px;
    margin: 0 auto;
  }
  @media (max-width: 632px) {
    width: 343px;
    margin: 0 auto;
  }
`;

class Body extends React.PureComponent {
  constructor(props) {
    super(props);
    const { t } = this.props;
    this.state = {
      // errorMessage: null,
      // isErrorLicense: false,
      isVisibleModalDialog: false,
      languages: null,
      timezones: null,
    };

    document.title = `${t("Payments")} – ${t("OrganizationName")}`;
  }
  mapCulturesToArray = (cultures, t) => {
    return cultures.map((culture) => {
      return { key: culture, label: t(`Culture_${culture}`) };
    });
  };
  componentDidMount() {
    const { getPortalCultures } = this.props;
    this.props.currentProductId !== "payments" &&
      this.props.setCurrentProductId("payments");
  }

  componentDidUpdate(prevProps) {
    if (this.props.currentProductId !== prevProps.currentProductId) {
      this.fetchData(this.props.currentProductId);
    }
  }

  onButtonClickUpload = (file) => {
    const { setLicense } = this.props;
    let fd = new FormData();
    fd.append("files", file);

    setLicense(null, fd).catch((e) =>
      this.setState({
        // errorMessage: e,
        // isErrorLicense: true,
        isVisibleModalDialog: true,
      })
    );
  };
  onButtonClickBuy = (e) => {
    window.open(e.target.value, "_blank");
  };
  onCloseModalDialog = () => {
    this.setState({
      isVisibleModalDialog: false,
      // errorMessage: null,
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
      culture,
    } = this.props;
    const {
      isVisibleModalDialog,
      languages,
      select,
      selectLanguage,
    } = this.state;
    // console.log(this.state.selectLanguage);
    return !isLoaded ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBody>
        <HeaderContainer
          t={t}
          dateExpires={dateExpires}
          languages={languages}
          culture={culture}
          select={select}
          createPortals={createPortals}
          selectLanguage={selectLanguage}
        />
        <AdvantagesContainer t={t} />
        <ModalDialogContainer
          t={t}
          isVisibleModalDialog={isVisibleModalDialog}
          onCloseModalDialog={this.onCloseModalDialog}
        />
        <ButtonContainer
          t={t}
          buyUrl={buyUrl}
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
    culture: state.auth.settings.culture,
  };
}
export default connect(mapStateToProps, {
  setLicense,
  setCurrentProductId,
  // getPortalCultures,
})(withRouter(PaymentsEnterprise));
