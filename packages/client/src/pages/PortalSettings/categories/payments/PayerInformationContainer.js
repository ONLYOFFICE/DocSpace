import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { HelpButton, Link } from "@docspace/components";
import Avatar from "@docspace/components/avatar";

const StyledContainer = styled.div`
  display: flex;
  background: ${(props) => props.theme.client.settings.payment.backgroundColor};
  min-height: 72px;
  padding: 16px;
  box-sizing: border-box;
  margin-top: 16px;

  .change-payer {
    margin-left: 3px;
  }

  .payer-info_avatar {
    margin-right: 16px;
  }
  .payer-info {
    margin-right: 3px;
  }
  .payer-info_wrapper {
    height: max-content;
    display: grid;
    grid-template-columns: 1fr;
    grid-template-rows: max-content max-content;
    grid-gap: 4px;

    .payer-info_description {
      display: flex;
      p {
        margin-right: 3px;
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
  user,
  accountLink,
  isPayer,
  payerInfo,
  payerEmail,
}) => {
  const { t } = useTranslation("Payments");

  const email = payerEmail;

  const isLinkAvailable = user.isOwner || isPayer;

  const renderTooltip = (
    <HelpButton
      iconName={"/static/images/help.react.svg"}
      tooltipContent={
        <>
          <Text isBold>{t("Payer")}</Text>
          <Text>{t("PayerDescription")}</Text>
        </>
      }
    />
  );

  const unknownPayerInformation = (
    <div>
      <Text as="span" fontSize="13px" isBold noSelect>
        {t("InvalidEmail")}
        {"."}
      </Text>
      {isLinkAvailable ? (
        <Link
          noSelect
          fontWeight={600}
          href={accountLink}
          className="change-payer"
          color={theme.client.settings.payment.linkColor}
        >
          {t("ChangePayer")}
        </Link>
      ) : (
        <Text as="span" fontSize="13px" isBold className="change-payer">
          {t("OwnerCanChangePayer")}
          {"."}
        </Text>
      )}
    </div>
  );

  const payerInformation = (
    <>
      {isLinkAvailable ? (
        <Link
          noSelect
          fontWeight={600}
          href={accountLink}
          className="payer-info_account-link"
          color={theme.client.payments.linkColor}
        >
          {t("StripeCustomerPortal")}
        </Link>
      ) : (
        <Link
          fontWeight={600}
          href={`mailto:${email}`}
          color={theme.client.payments.linkColor}
        >
          {email}
        </Link>
      )}
    </>
  );

  const payerName = (
    <Text fontWeight={600} noSelect fontSize={"14px"}>
      {payerInfo ? (
        payerInfo.displayName
      ) : (
        <Trans t={t} i18nKey="UserNotFound" ns="Payments">
          User
          <Text as="span" color={"#F21C0E"} fontWeight={600}>
            {{ email }}
          </Text>
          is not found
        </Trans>
      )}
    </Text>
  );

  return (
    <StyledContainer style={style} theme={theme}>
      <div className="payer-info_avatar">
        <Avatar
          size={"base"}
          source={user.avatar}
          userName={user.displayName}
        />
      </div>

      <div className="payer-info_wrapper">
        <div className="payer-info_description">
          {payerName}

          <Text as="span" className="payer-info">
            {" "}
            {" (" + t("Payer") + ") "}
          </Text>

          {renderTooltip}
        </div>

        {!payerInfo ? unknownPayerInformation : payerInformation}
      </div>
    </StyledContainer>
  );
};

export default inject(({ auth, payments }) => {
  const { userStore } = auth;
  const { accountLink } = payments;

  const { user } = userStore;

  return {
    theme: auth.settingsStore.theme,
    user,
    accountLink,
  };
})(observer(PayerInformationContainer));
