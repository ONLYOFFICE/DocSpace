import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { HelpButton, Link } from "@docspace/components";
import Avatar from "@docspace/components/avatar";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const StyledContainer = styled.div`
  display: flex;
  background: ${(props) => props.theme.client.settings.payment.backgroundColor};
  min-height: 72px;
  padding: 16px;
  box-sizing: border-box;
  margin-top: 16px;
  border-radius: 6px;
  .change-payer,
  .payer-info {
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
      p {
        margin-right: 3px;
      }
      div {
        display: inline-block;
        margin: auto 0;
        height: 14px;
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
        <ColorTheme
          noSelect
          fontWeight={600}
          href={accountLink}
          className="change-payer"
          tag="a"
          themeId={ThemeType.Link}
          target="_blank"
        >
          {t("ChangePayer")}
        </ColorTheme>
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
        <ColorTheme
          noSelect
          fontWeight={600}
          href={accountLink}
          className="payer-info_account-link"
          tag="a"
          themeId={ThemeType.Link}
          target="_blank"
        >
          {t("StripeCustomerPortal")}
        </ColorTheme>
      ) : (
        <ColorTheme
          fontWeight={600}
          href={`mailto:${email}`}
          tag="a"
          themeId={ThemeType.Link}
        >
          {email}
        </ColorTheme>
      )}
    </>
  );

  const payerName = () => {
    let emailUnfoundedUser = email;

    if (email) emailUnfoundedUser = "«" + emailUnfoundedUser + "»";

    return (
      <Text as="span" fontWeight={600} noSelect fontSize={"14px"}>
        {payerInfo ? (
          payerInfo.displayName
        ) : (
          <Trans t={t} i18nKey="UserNotFound" ns="Payments">
            User
            <Text
              as="span"
              color={theme.client.settings.payment.warningColor}
              fontWeight={600}
            >
              {{ email: emailUnfoundedUser }}
            </Text>
            is not found
          </Trans>
        )}
      </Text>
    );
  };

  const avatarUrl = payerInfo ? { source: payerInfo.avatar } : {};

  return (
    <StyledContainer style={style} theme={theme}>
      <div className="payer-info_avatar">
        <Avatar
          size={"base"}
          {...avatarUrl}
          isDefaultSource
          userName={payerInfo?.displayName}
        />
      </div>

      <div className="payer-info_wrapper">
        <div className="payer-info_description">
          {payerName()}

          <Text as="span" className="payer-info">
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
  const { userStore, settingsStore } = auth;
  const { accountLink } = payments;
  const { theme } = settingsStore;

  const { user } = userStore;

  return {
    theme,
    user,
    accountLink,
  };
})(observer(PayerInformationContainer));
