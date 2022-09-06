import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { HelpButton, Link } from "@docspace/components";
import Avatar from "@docspace/components/avatar";
import UnknownPayerInformationContainer from "./sub-components/UnknownPayerInformationContainer";

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
  .payer-info {
    margin-right: 3px;
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
  payer,
  payerInfo,
}) => {
  const { t } = useTranslation("Payments");

  const email = payerInfo ? payerInfo.email : t("InvalidEmail");

  const isLinkAvailable = user.isOwner || payer;

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
    <StyledContainer style={style} theme={theme}>
      <div className="payer-info_avatar">
        <Avatar size={"min"} source={user.avatar} userName={user.displayName} />
      </div>

      <div className="payer-info_wrapper">
        <div className="payer-info_description">
          <Text fontWeight={600} noSelect>
            {payerInfo ? (
              payerInfo.displayName
            ) : (
              <Trans t={t} i18nKey="UserNotFound" ns="Payments">
                {{ email: "test email" }}
              </Trans>
            )}
          </Text>
          <Text as="span" className="payer-info">
            {" "}
            {" (" + t("Payer") + ") "}
          </Text>

          {renderTooltip()}
        </div>

        {!payerInfo ? (
          <UnknownPayerInformationContainer
            email={email}
            t={t}
            isLinkAvailable={isLinkAvailable}
            accountLink={accountLink}
            theme={theme}
          />
        ) : (
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
        )}
      </div>
    </StyledContainer>
  );
};

export default inject(({ auth, payments }) => {
  const { quota, userStore } = auth;
  const { accountLink } = payments;

  const { user } = userStore;

  return {
    quota,
    theme: auth.settingsStore.theme,
    user,
    accountLink,
  };
})(observer(PayerInformationContainer));
