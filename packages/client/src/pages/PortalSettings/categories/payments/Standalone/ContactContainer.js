import React from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

import { StyledContactComponent } from "./StyledComponent";
const ContactContainer = (props) => {
  const { helpUrl, salesEmail, t, theme } = props;

  return (
    <StyledContactComponent>
      <div className="payments_contact">
        <Text
          fontWeight={600}
          color={theme.client.settings.payment.contactContainer.textColor}
        >
          <Trans i18nKey="PurchaseQuestions" ns="Payments" t={t}>
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
          <Trans i18nKey="TechAssistance" ns="Payments" t={t}>
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
  );
};

export default inject(({ auth, payments }) => {
  const { settingsStore } = auth;
  const { helpUrl, salesEmail } = payments;
  const { theme } = settingsStore;
  return { helpUrl, salesEmail, theme };
})(withRouter(observer(ContactContainer)));
