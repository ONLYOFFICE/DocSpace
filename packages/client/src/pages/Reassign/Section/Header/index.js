import React, { useCallback } from "react";
import IconButton from "@docspace/components/icon-button";
import Headline from "@docspace/common/components/Headline";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";

const Wrapper = styled.div`
  display: grid;
  grid-template-columns: auto 1fr auto auto;
  align-items: center;

  .arrow-button {
    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }
`;

const textStyle = {
  marginLeft: "16px",
};

const SectionHeaderContent = (props) => {
  const { history } = props;
  const { t } = useTranslation("People");

  const onClickBack = useCallback(() => {
    history.push(combineUrl(AppServerConfig.proxyURL, config.homepage));
  }, [history]);

  return (
    <Wrapper>
      <div style={{ width: "17px" }}>
        <IconButton
          iconName="/static/images/arrow.path.react.svg"
          // color="#A3A9AE"
          size="17"
          // hoverColor="#657077"
          isFill={true}
          onClick={onClickBack}
          className="arrow-button"
        />
      </div>
      <Headline type="content" truncate={true} style={textStyle}>
        {/* {profile.displayName}
        {profile.isLDAP && ` (${t('Translations:LDAPLbl')})`}
        -  */}
        {t("ReassignmentData")}
      </Headline>
    </Wrapper>
  );
};

export default withRouter(SectionHeaderContent);
