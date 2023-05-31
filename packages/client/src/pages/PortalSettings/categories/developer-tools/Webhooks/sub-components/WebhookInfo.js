import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { Base } from "@docspace/components/themes";

import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

import { useTranslation } from "react-i18next";

const InfoWrapper = styled.div`
  margin-bottom: 25px;
`;

const InfoText = styled(Text)`
  max-width: 660px;
  white-space: break-spaces;
  margin: 0 0 8px 0;

  color: ${(props) => (props.theme.isBase ? "#657077" : "rgba(255, 255, 255, 0.6)")};
  &:hover {
    color: ${(props) => (props.theme.isBase ? "#657077" : "rgba(255, 255, 255, 0.6)")};
  }
`;

InfoText.defaultProps = { theme: Base };

const StyledGuideLink = styled(Link)`
  color: ${(props) => (props.theme.isBase ? "#316DAA" : "#4781D1")};
  &:hover {
    color: ${(props) => (props.theme.isBase ? "#316DAA" : "#4781D1")};
  }
`;

StyledGuideLink.defaultProps = { theme: Base };

const WebhookInfo = (props) => {
  const { t } = useTranslation(["Webhooks"]);
  const { webhooksGuideUrl } = props;

  return (
    <InfoWrapper>
      <InfoText as="p">{t("WebhooksInfo")}</InfoText>
      <StyledGuideLink fontWeight={600} isHovered type="page" href={webhooksGuideUrl}>
        {t("WebhooksGuide")}
      </StyledGuideLink>
    </InfoWrapper>
  );
};

export default inject(({ settingsStore }) => {
  const { webhooksGuideUrl } = settingsStore;

  return {
    webhooksGuideUrl,
  };
})(observer(WebhookInfo));
