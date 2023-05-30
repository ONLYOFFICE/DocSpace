import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import Text from "@docspace/components/text";

const StyledContactContainer = styled.div`
  display: flex;
  width: 100%;
  a {
    margin-left: 4px;
  }
`;

const ContactContainer = ({ t, salesEmail }) => {
  return (
    <StyledContactContainer>
      {salesEmail && (
        <Text as="span" noSelect fontWeight={600}>
          {t("ContactUs")}
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontWeight="600"
            href={`mailto:${salesEmail}`}
          >
            {salesEmail}
          </ColorTheme>
        </Text>
      )}
    </StyledContactContainer>
  );
};

export default inject(({ payments, auth }) => {
  const { salesEmail } = payments;
  return {
    salesEmail,
    theme: auth.settingsStore.theme,
  };
})(observer(ContactContainer));
