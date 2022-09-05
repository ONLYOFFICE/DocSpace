import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import SocialButton from "@docspace/components/social-button";

import { hugeMobile } from "@docspace/components/utils/device";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .buttons {
    display: flex;
    flex-direction: column;
    gap: 12px;

    .buttons-row {
      display: flex;
      gap: 20px;

      @media ${hugeMobile} {
        flex-direction: column;
      }
    }
  }
`;

const SocialNetworks = () => {
  const { t } = useTranslation(["Profile", "Common"]);

  return (
    <StyledWrapper>
      <Text fontSize="16px" fontWeight={700}>
        {t("ConnectSocialNetworks")}
      </Text>
      <div className="buttons">
        <div className="buttons-row">
          <SocialButton
            iconName="static/images/share.linkedin.react.svg"
            label={t("Common:SignInWithLinkedIn")}
          />
          <SocialButton
            iconName="static/images/share.google.react.svg"
            label={t("Common:SignInWithGoogle")}
          />
        </div>
        <div className="buttons-row">
          <SocialButton
            iconName="static/images/share.twitter.react.svg"
            label={t("Common:SignInWithTwitter")}
          />
          <SocialButton
            iconName="static/images/share.facebook.react.svg"
            label={t("Common:SignInWithFacebook")}
          />
        </div>
      </div>
    </StyledWrapper>
  );
};

export default SocialNetworks;
