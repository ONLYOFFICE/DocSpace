import React, { useEffect } from "react";
import { PageLayout, utils } from "asc-web-common";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { Loader, Button, Text, Link } from "asc-web-components";
import styled from "styled-components";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { store, history } from "asc-web-common";
import PropTypes from "prop-types";
const { changeLanguage } = utils;
const supportLinkPurchaseQuestions = "sales@onlyoffice.com";
const supportLinkTechnicalIssues = "https://helpdesk.onlyoffice.com";
const { getPortalSettings, setIsLoaded } = store.auth.actions;
const BodyStyle = styled.div`
  margin: 0 auto;
  width: 920px;
  .contact-emails {
    position: static;
    width: 920px;
    margin-bottom: 11px;
  }
  .contact-emails_link {
    color: #316daa;
  }
`;

const BodyAdvantages = styled.div`
  margin-top: 56px;

  display: flex;
  flex-direction: column;
  padding: 32px 32px 32px 0px;
  width: 888px;
  height: 247px;
  position: static;

  background: #f8f9f9;
  border-radius: 6px 6px 0px 0px;

  .payments-icon-enterprise {
    position: absolute;
    margin-left: 666px;
  }
  .first_icon {
    margin: 64px 864px 193px 32px;
  }
  .second_icon {
    margin: 104px 864px 193px 32px;
  }
  .third_icon {
    margin: 144px 864px 193px 32px;
  }
  .four_icon {
    margin: 184px 864px 193px 32px;
  }
  .five_icon {
    margin: 224px 864px 193px 32px;
  }
  .payments-body {
    margin-right: 609px;
    margin-bottom: 196px;
    margin-left: 64px;
    position: absolute;
    font-weight: bold;
    font-size: 13px;
    color: #555f65;
  }
  .first-advantages {
    margin-top: 67px;
    width: 247px;
  }
  .second-advantages {
    margin-top: 107px;
    width: 278px;
  }
  .third-advantages {
    margin-top: 147px;
    width: 124px;
  }
  .four-advantages {
    margin-top: 187px;
    width: 366px;
  }
  .five-advantages {
    margin-top: 227px;
    width: 276px;
  }
  .advantages-enterprise {
    position: static;
    width: 856px;
    height: 30px;
    left: 0px;
    top: 0px;

    font-family: Open Sans;
    font-style: normal;
    font-weight: bold;
    font-size: 22px;
    line-height: 30px;

    color: #333333;
    flex: none;
    order: 0;
    margin: 0px 33px;
  }
`;
const HeaderStylePayments = styled.div`
  .payments-header {
    margin-top: 50px;
    height: 32px;
    font-style: normal;
    font-weight: bold;
    font-size: 27px;
    line-height: 32px;
    color: #333333;
    margin-bottom: 8px;
  }
  .payments-header-additional_support {
    margin-top: 8px;
  }
  .payments-header-additional_portals {
    margin-top: 12px;
  }
`;
const LicenseBlockStyle = styled.div`
  position: relative;
  background: #edf2f7;
  height: 108px;
  margin-bottom: 17px;
  .button-payments-enterprise {
    padding: 0px;
    background: #2da7db;
    color: white;
    height: 44px;
    font-style: SemiBold;
    font-size: 16px;
    text-align: center;
  }
  .button-buy {
    width: 107px;
    margin: 32px 781px 32px 32px;
  }
  .button-upload {
    position: absolute;
    width: 153px;
    margin: 32px 612px 32px 155px;
  }
`;
const Body = ({ modules, match, standAloneMode, isLoaded }) => {
  const { t } = useTranslation("translation", { i18n });

  const onButtonClickBuy = (e) => {
    getPortalSettings(store.dispatch)
      .then(() => store.dispatch(setIsLoaded(true)))
      .catch((e) => history.push(`/login/error=${e}`));
  };

  useEffect(() => {
    changeLanguage(i18n);
    document.title = `${t("Payments")}`;
  }, [t]);

  return !isLoaded ? (
    <Loader className="pageLoader" type="rombs" size="40px" />
  ) : (
    <BodyStyle>
      <HeaderStylePayments>
        <Text className="payments-header">{t("Using")}</Text>
        <Text className="payments-header-additional_support">
          {t("SubscriptionAndUpdatesExpires")} 1 march 2021.
        </Text>
        <Text className="payments-header-additional_portals">
          Порталы, созданные вами: 1/2
        </Text>
      </HeaderStylePayments>
      <BodyAdvantages>
        <Text className="advantages-enterprise">{t("SubscriptionGet")}</Text>
        <Text className="payments-body first-advantages">
          {t("OnlyOfficeEditor")}
        </Text>
        <Text className="payments-body second-advantages">
          {t("PrivateRooom")}
        </Text>
        <Text className="payments-body third-advantages">
          {t("WebEditors")}
        </Text>
        <Text className="payments-body four-advantages">
          {t("FunctionalityAndSecurity")}
        </Text>
        <Text className="payments-body five-advantages">
          {t("ProfessionalTechSupport")}
        </Text>
        <img
          className="payments-icon-enterprise"
          src="images/payments_enterprise.png"
          width="222px"
          height="247px"
          alt="Icon"
        />
        <img
          className="payments-icon-enterprise first_icon"
          src="images/payments_enterprise_icon_first.png"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <img
          className="payments-icon-enterprise second_icon"
          src="images/payments_enterprise_icon_first.png"
          width="24px"
          height="23px"
          alt="Icon"
        />

        <img
          className="payments-icon-enterprise 
          third_icon"
          src="images/payments_enterprise_icon_four.png"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <img
          className="payments-icon-enterprise 
          four_icon"
          src="images/payments_enterprise_icon_second.png"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <img
          className="payments-icon-enterprise 
          five_icon"
          src="images/payments_enterprise_icon_third.png"
          width="24px"
          height="23px"
          alt="Icon"
        />
      </BodyAdvantages>
      <LicenseBlockStyle>
        <Button
          className="button-payments-enterprise button-upload"
          label="Upload license"
        />
        <Button
          className="button-payments-enterprise button-buy"
          label="Buy now"
          onClick={onButtonClickBuy}
        />
      </LicenseBlockStyle>
      <Text className="contact-emails">
        {t("PurchaseQuestions")}{" "}
        <Link
          className="contact-emails_link"
          href={`mailto:${supportLinkPurchaseQuestions}`}
        >
          {supportLinkPurchaseQuestions}
        </Link>
      </Text>
      <Text className="contact-emails">
        {t("TechnicalIssues")}{" "}
        <Link
          className="contact-emails_link"
          href={`${supportLinkTechnicalIssues}`}
        >
          {supportLinkTechnicalIssues}
        </Link>
      </Text>
    </BodyStyle>
  );
};

const PaymentsEnterprise = (props) => (
  <PageLayout>
    <PageLayout.SectionBody>
      <Body {...props} />
    </PageLayout.SectionBody>
  </PageLayout>
);

PaymentsEnterprise.propTypes = {
  modules: PropTypes.array.isRequired,
  standAloneMode: PropTypes.bool,
  isLoaded: PropTypes.bool,
};

function mapStateToProps(state) {
  return {
    standAloneMode: state.auth.settings.standAloneMode,
    modules: state.auth.modules,
    isLoaded: state.auth.isLoaded,
  };
}
export default connect(mapStateToProps)(withRouter(PaymentsEnterprise));
