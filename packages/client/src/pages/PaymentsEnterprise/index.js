import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
import Loader from "@docspace/components/loader";
import { tablet, size } from "@docspace/components/utils/device";
import HeaderContainer from "./sub-components/headerContainer";
import AdvantagesContainer from "./sub-components/advantagesContainer";
import ButtonContainer from "./sub-components/buttonContainer";
import ContactContainer from "./sub-components/contactContainer";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

const StyledBody = styled.div`
  margin: 0 auto;
  max-width: 920px;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(4, min-content);
  overflow-wrap: anywhere;
  margin-top: 40px;

  ${isMobile &&
  `
      margin-top: 56px;
    `}

  @media ${tablet} {
    max-width: ${size.smallTablet}px;
  }
  @media (max-width: 632px) {
    min-width: 343px;
    ${!isMobile && `margin-top: 0;`}
  }
`;

class Body extends React.Component {
  constructor(props) {
    super(props);
    const { t } = this.props;

    setDocumentTitle(`${t("PaymentsTitle")}`);
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
const PaymentsWrapper = withTranslation("PaymentsEnterprise")(Body);
const PaymentsEnterprise = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <PaymentsWrapper {...props} />
      </Section.SectionBody>
    </Section>
  );
};

PaymentsEnterprise.propTypes = {
  isLoaded: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const { isLoaded, settingsStore } = auth;
  const { getSettingsPayment } = payments;
  const { setCurrentProductId } = settingsStore;
  return {
    isLoaded,
    setCurrentProductId,
    getSettingsPayment,
  };
})(withRouter(observer(PaymentsEnterprise)));
