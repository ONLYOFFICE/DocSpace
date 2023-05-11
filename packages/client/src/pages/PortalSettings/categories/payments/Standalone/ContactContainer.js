import React from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";
import Text from "@docspace/components/text";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import { StyledContactComponent } from "./StyledComponent";
const ContactContainer = (props) => {
  const { helpUrl, salesEmail, t } = props;

  return (
    <StyledContactComponent>
      <div className="payments_contact">
        <Text fontWeight={600}>
          <Trans i18nKey="PurchaseQuestions" ns="Payments" t={t}>
            Ask purchase questions at
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

      <div className="payments_contact">
        <Text fontWeight={600}>
          <Trans i18nKey="TechAssistance" ns="Payments" t={t}>
            Get tech assistance
            <ColorTheme
              target="_blank"
              tag="a"
              themeId={ThemeType.Link}
              fontWeight="600"
              href={helpUrl}
            >
              {{ helpUrl }}
            </ColorTheme>
          </Trans>
        </Text>
      </div>
    </StyledContactComponent>
  );
};

export default inject(({ payments }) => {
  const { helpUrl, salesEmail } = payments;

  return { helpUrl, salesEmail };
})(withRouter(observer(ContactContainer)));
