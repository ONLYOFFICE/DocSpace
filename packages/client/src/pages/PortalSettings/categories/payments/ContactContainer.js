import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import Text from "@docspace/components/text";

const StyledContactContainer = styled.div`
  display: flex;
  width: max-content;
  p {
    margin-right: 4px;
  }
`;

const ContactContainer = ({ t, salesEmail, theme }) => {
  return (
    <StyledContactContainer>
      <Text noSelect fontWeight={600}>
        {t("ContactUs")}
      </Text>
      <ColorTheme
        tag="a"
        themeId={ThemeType.Link}
        fontWeight="600"
        href={`mailto:${salesEmail}`}
      >
        {salesEmail}
      </ColorTheme>
    </StyledContactContainer>
  );
};

export default inject(({ payments, auth }) => {
  const { salesEmail } = payments;
  return {
    salesEmail,
    theme: auth.settingsStore.theme,
  };
})(withRouter(observer(ContactContainer)));
