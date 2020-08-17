import React, { useEffect } from "react";

import { PageLayout, utils } from "asc-web-common";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { Loader, toastr, Text } from "asc-web-components";
import styled from "styled-components";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import version from "../../../../package.json";
const { changeLanguage } = utils;

const BodyAdvantages = styled.div`
  margin: 202px 232px 288px 388px;
  position: relative;
  display: flex;
  flex-direction: column;
  padding: 32px 32px 32px 0px;

  position: static;
  width: 888px;
  height: 346px;

  background: #f8f9f9;
  border-radius: 6px 6px 0px 0px;

  .payments-icon-enterprise {
    position: absolute;
    margin-left: 577px;
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
const VersionStyle = styled.div`
  background: #ffff;
  padding: 8px 0px 20px 0px;
`;
const BodyStyle = styled.div`
  background: #ffff;
`;
const Body = ({ modules, match, isLoaded }) => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    changeLanguage(i18n);
    document.title = `${t("Payments")}`;
  }, [t]);

  return !isLoaded ? (
    <Loader className="pageLoader" type="rombs" size="40px" />
  ) : (
    <BodyStyle>
      <VersionStyle>
        <Text className="text_style" fontSize="14px" color="#A3A9AE">
          {`${t("AboutCompanyVersion")}: ${version.version}`}
        </Text>
      </VersionStyle>
      <BodyAdvantages>
        <p className="advantages-enterprise">
          Активируйте ONLYOFFICE и получите:
        </p>
        <img
          className="payments-icon-enterprise"
          src="images/payments_enterprise.png"
          width="311px"
          height="346px"
          alt="Logo"
        />
      </BodyAdvantages>
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
function mapStateToProps(state) {
  return {
    modules: state.auth.modules,
    isLoaded: state.auth.isLoaded,
  };
}
export default connect(mapStateToProps)(withRouter(PaymentsEnterprise));
