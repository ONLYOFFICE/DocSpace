import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import Text from "@docspace/components/text";
import ArrowRightIcon from "@docspace/client/public/images/arrow.right.react.svg";
import { StyledArticlePaymentAlert } from "../styled-article";
import styled from "styled-components";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";
const StyledArrowRightIcon = styled(ArrowRightIcon)`
  margin: auto 0;
  path {
    fill: ${(props) => props.color};
  }
`;

const PROXY_BASE_URL = combineUrl(AppServerConfig.proxyURL, "/portal-settings");

const ArticlePaymentAlert = ({ pricePerManager, isFreeTariff, theme }) => {
  const { t } = useTranslation("Payments");

  const onClick = () => {
    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    history.push(paymentPageUrl);
  };

  return (
    <StyledArticlePaymentAlert
      onClick={onClick}
      isFreeTariff={isFreeTariff}
      theme={theme}
    >
      <div>
        <Text className="article-payment_border">{t("FreeStartupPlan")}</Text>
        <Text fontWeight={600}>{t("ActivateBusinessPlan")}</Text>
        <Text noSelect fontSize={"12px"}>
          <Trans t={t} i18nKey="StartPrice" ns="Payments">
            {{ price: pricePerManager }}
          </Trans>
        </Text>
      </div>

      <StyledArrowRightIcon />
    </StyledArticlePaymentAlert>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { pricePerManager } = auth;
    const { theme } = auth.settingsStore;

    return { pricePerManager, theme };
  })(observer(ArticlePaymentAlert))
);
