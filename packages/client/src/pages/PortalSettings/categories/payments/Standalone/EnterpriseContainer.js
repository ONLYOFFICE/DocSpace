import React from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import {
  StyledEnterpriseComponent,
  StyledButtonComponent,
} from "./StyledComponent";

const EnterpriseContainer = (props) => {
  const { buyUrl, salesEmail, t } = props;
  const onClickBuy = () => {
    window.open(buyUrl, "_blank");
  };

  return (
    <StyledEnterpriseComponent>
      <Text fontWeight={700} fontSize={"16px"}>
        {t("EnterpriseRenewSubscription")}
      </Text>
      <div className="payments_subscription">
        <Text isBold fontSize="14px" as="span">
          {t("EnterpriseEdition")}{" "}
        </Text>
        <Text as="span" fontWeight={400} fontSize="14px">
          {t("EnterpriseSubscriptionExpires", {
            finalDate: "Tuesday, December 19, 2023.",
          })}
        </Text>
      </div>
      <Text
        fontWeight={400}
        fontSize="14px"
        className="payments_renew-subscription"
      >
        {t("EnterpriseRenewal")}
      </Text>
      <StyledButtonComponent>
        <Button
          label={t("BuyNow")}
          size={"small"}
          primary
          onClick={onClickBuy}
        />
      </StyledButtonComponent>

      <div className="payments_support">
        <Text>
          <Trans i18nKey="EnterprisePersonalRenewal" ns="Payments" t={t}>
            To get your personal renewal terms, contact your dedicated manager
            or write us at
            <ColorTheme
              fontWeight="600"
              target="_blank"
              tag="a"
              href={`mailto:${salesEmail}`}
              themeId={ThemeType.Link}
            >
              {{ email: salesEmail }}
            </ColorTheme>
          </Trans>
        </Text>
      </div>
    </StyledEnterpriseComponent>
  );
};

export default inject(({ payments }) => {
  const { buyUrl, salesEmail } = payments;

  return { buyUrl, salesEmail };
})(withRouter(observer(EnterpriseContainer)));
