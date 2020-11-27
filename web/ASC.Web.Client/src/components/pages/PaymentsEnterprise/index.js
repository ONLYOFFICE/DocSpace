import React, { useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import PageLayout from "@appserver/common/src/components/PageLayout";
import utils from "@appserver/common/src/utils";
import store from "@appserver/common/src/store";
import Loader from "@appserver/components/src/components/loader";
import { tablet, size } from "@appserver/components/src/utils/device";
import HeaderContainer from "./sub-components/headerContainer";
import AdvantagesContainer from "./sub-components/advantagesContainer";
import ButtonContainer from "./sub-components/buttonContainer";
import ContactContainer from "./sub-components/contactContainer";
import { getSettingsPayment } from "../../../store/payments/actions";
import { createI18N } from "../../../helpers/i18n";
import { setDocumentTitle } from "../../../helpers/utils";
const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});
const { setCurrentProductId } = store.auth.actions;
const { changeLanguage } = utils;

const StyledBody = styled.div`
  margin: 0 auto;
  max-width: 920px;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(4, min-content);
  overflow-wrap: anywhere;
  margin-top: 40px;
  @media ${tablet} {
    max-width: ${size.smallTablet}px;
  }
  @media (max-width: 632px) {
    min-width: 343px;
    margin-top: 0;
  }
`;

class Body extends React.Component {
  constructor(props) {
    super(props);
    const { t } = this.props;

    setDocumentTitle(`${t("Payments")}`);
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

  render() {
    const { isLoaded } = this.props;

    return !isLoaded ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBody>
        <HeaderContainer />
        <AdvantagesContainer />
        <ButtonContainer />
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

function mapStateToProps({ auth }) {
  const { isLoaded } = auth;
  return {
    isLoaded,
  };
}
export default connect(mapStateToProps, {
  setCurrentProductId,
  getSettingsPayment,
})(withRouter(PaymentsEnterprise));
