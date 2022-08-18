import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { HelpButton, Link } from "@docspace/components";
import toastr from "client/toastr";

const StyledContainer = styled.div`
  display: flex;
  background: #f8f9f9;
  min-height: 72px;
  padding: 16px;
  box-sizing: border-box;
  margin-top: 16px;

  .payer-info_avatar {
    margin-right: 16px;
  }
  .payer-info_wrapper {
    height: max-content;
    display: grid;
    grid-template-columns: 1fr;
    grid-template-rows: max-content max-content;
    grid-gap: 6px;

    .payer-info_description {
      display: flex;
      p {
        margin-right: 8px;
      }
      div {
        margin: auto 0;
      }
    }
    .payer-info_account-link {
      cursor: pointer;
      text-decoration: underline;
    }
  }
`;

const PayerInformationContainer = ({
  style,
  theme,
  rights,
  getPaymentAccount,
}) => {
  const { t } = useTranslation("Payments");

  const payerName = `${"Test Name" + " (" + t("Payer") + ")"}`;

  const email = "example email";

  const onClick = async () => {
    try {
      const accountLink = await getPaymentAccount();
      if (accountLink) {
        window.open(accountLink, "_self");
      }
    } catch (e) {
      toastr.error(e);
    }
  };

  const isLinkAvailable = rights === "3" ? false : true;

  const renderTooltip = () => {
    return (
      <>
        <HelpButton
          iconName={"/static/images/help.react.svg"}
          tooltipContent={
            <>
              <Text isBold>{t("Payer")}</Text>
              <Text>{t("PayerDescription")}</Text>
            </>
          }
        />
      </>
    );
  };
  return (
    <StyledContainer style={style} className="current-tariff" theme={theme}>
      <div className="payer-info_avatar">
        <Text isBold noSelect>
          {"AvatarPlace"}:
        </Text>
      </div>

      <div className="payer-info_wrapper">
        <div className="payer-info_description">
          <Text fontWeight={600} noSelect>
            {payerName}
          </Text>
          {renderTooltip()}
        </div>
        {isLinkAvailable ? (
          <Text
            fontWeight={600}
            noSelect
            onClick={onClick}
            className="payer-info_account-link"
            color={theme.client.payments.linkColor}
          >
            {t("StripeCustomerPortal")}
          </Text>
        ) : (
          <Link
            fontWeight={600}
            href={`mailto:${email}`}
            color={theme.client.payments.linkColor}
          >
            {email}
          </Link>
        )}
      </div>
    </StyledContainer>
  );
};

export default inject(({ auth, payments }) => {
  const { quota, portalQuota } = auth;
  const { getPaymentAccount } = payments;

  //const rights = "2";
  //const rights = "3";
  const rights = "1";

  return {
    quota,
    portalQuota,
    theme: auth.settingsStore.theme,
    rights,
    getPaymentAccount,
  };
})(observer(PayerInformationContainer));
