import React from "react";

import { inject, observer } from "mobx-react";
import { Trans, useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

import { StyledContactComponent } from "./StyledComponent";
const ContactContainer = (props) => {
  const { t } = useTranslation("PaymentsEnterprise");

  const { helpUrl, salesEmail, theme, isCommunity } = props;

  const officialWebsiteUrl = "https://www.onlyoffice.com/for-enterprises.aspx";
  const demonstrationUrl = "https://www.onlyoffice.com/demo-order.aspx";

  return (
    <>
      {isCommunity && (
        <StyledContactComponent>
          <div className="payments_contact">
            <Text
              fontWeight={600}
              color={theme.client.settings.payment.contactContainer.textColor}
            >
              <Trans
                i18nKey="UpgradeToProBannerInformationAboutShort"
                ns="PaymentsEnterprise"
                t={t}
              >
                Learn more about Enterprise Edition
                <Link
                  target="_blank"
                  tag="a"
                  fontWeight="600"
                  href={officialWebsiteUrl}
                  color={
                    theme.client.settings.payment.contactContainer.linkColor
                  }
                >
                  on the official website
                </Link>
              </Trans>
            </Text>
          </div>
          <div className="payments_contact">
            <Text
              fontWeight={600}
              color={theme.client.settings.payment.contactContainer.textColor}
            >
              <Trans
                i18nKey="UpgradeToProBannerInformationDemo"
                ns="PaymentsEnterprise"
                t={t}
              >
                Request demonstration
                <Link
                  target="_blank"
                  tag="a"
                  fontWeight="600"
                  href={demonstrationUrl}
                  color={
                    theme.client.settings.payment.contactContainer.linkColor
                  }
                >
                  here
                </Link>
              </Trans>
            </Text>
          </div>
        </StyledContactComponent>
      )}
      <StyledContactComponent>
        <div className="payments_contact">
          <Text
            fontWeight={600}
            color={theme.client.settings.payment.contactContainer.textColor}
          >
            <Trans
              i18nKey="UpgradeToProBannerInformationPurchase"
              ns="PaymentsEnterprise"
              t={t}
            >
              Ask purchase questions at
              <Link
                fontWeight="600"
                target="_blank"
                tag="a"
                href={`mailto:${salesEmail}`}
                color={theme.client.settings.payment.contactContainer.linkColor}
              >
                {{ email: salesEmail }}
              </Link>
            </Trans>
          </Text>
        </div>
        <div className="payments_contact">
          <Text
            fontWeight={600}
            color={theme.client.settings.payment.contactContainer.textColor}
          >
            <Trans
              i18nKey="UpgradeToProBannerInformationSupport"
              ns="PaymentsEnterprise"
              t={t}
            >
              Get tech assistance
              <Link
                target="_blank"
                tag="a"
                fontWeight="600"
                href={helpUrl}
                color={theme.client.settings.payment.contactContainer.linkColor}
              >
                {{ helpUrl }}
              </Link>
            </Trans>
          </Text>
        </div>
      </StyledContactComponent>
    </>
  );
};

export default inject(({ auth, payments }) => {
  const { settingsStore, isCommunity } = auth;
  const { helpUrl, salesEmail } = payments;
  const { theme } = settingsStore;
  return { helpUrl, salesEmail, theme, isCommunity };
})(observer(ContactContainer));
