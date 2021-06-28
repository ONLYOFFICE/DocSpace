import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import { tablet, smallTablet } from "@appserver/components/utils/device";
import CatalogPinIcon from "../../../../../public/images/catalog.pin.react.svg";
import CatalogUnpinIcon from "../../../../../public/images/catalog.unpin.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

const StyledCatalogPinIcon = styled(CatalogPinIcon)`
  ${commonIconsStyles}
`;

const StyledCatalogUnpinIcon = styled(CatalogUnpinIcon)`
  ${commonIconsStyles}
`;

const StyledArticlePinPanel = styled.div`
  border-top: 1px solid #eceef1;
  height: 47px;
  min-height: 47px;
  display: none;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media ${tablet} {
    display: block;
    position: fixed;
    bottom: 0;
    width: 208px;
    z-index: 10;
    background-color: #f8f9f9;
  }

  @media ${smallTablet} {
    display: none;
  }

  div {
    display: flex;
    align-items: center;
    cursor: pointer;
    user-select: none;
    height: 100%;

    .icon-wrapper {
      width: 19px;
      height: 16px;
    }
    svg {
      margin-top: -1px;
    }

    span {
      margin-left: 6px;
      margin-top: -2px !important;
    }
  }
`;

const ArticlePinPanel = React.memo((props) => {
  //console.log("PageLayout ArticlePinPanel render");

  const { pinned, onPin, onUnpin, t } = props;
  const textStyles = {
    as: "span",
    color: "#555F65",
    fontSize: "14px",
    fontWeight: 600,
  };

  return (
    <StyledArticlePinPanel>
      {pinned ? (
        <div onClick={onUnpin}>
          <div className="icon-wrapper">
            <StyledCatalogUnpinIcon size="scale" />
          </div>
          <Text {...textStyles}>{t("Common:Unpin")}</Text>
        </div>
      ) : (
        <div onClick={onPin}>
          <div className="icon-wrapper">
            <StyledCatalogPinIcon size="scale" />
          </div>
          <Text {...textStyles}>{t("Common:Pin")}</Text>
        </div>
      )}
    </StyledArticlePinPanel>
  );
});

ArticlePinPanel.displayName = "ArticlePinPanel";

ArticlePinPanel.propTypes = {
  pinned: PropTypes.bool,
  pinText: PropTypes.string,
  onPin: PropTypes.func,
  unpinText: PropTypes.string,
  onUnpin: PropTypes.func,
};

const ArticlePinPanelWrapper = withTranslation("Common")(ArticlePinPanel);

export default ArticlePinPanelWrapper;
