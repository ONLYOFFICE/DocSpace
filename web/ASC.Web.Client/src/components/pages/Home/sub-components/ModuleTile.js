import React from "react";
import PropTypes from "prop-types";
import Text from "@appserver/components/text";
import StyledModuleTile from "./StyledModuleTile";
import {
  getLink,
  checkIfModuleOld,
  onItemClick,
} from "@appserver/studio/src/helpers/utils";
import StyledExternalLinkIcon from "@appserver/studio/src/components/StyledExternalLinkIcon";

const ModuleTile = (props) => {
  const { title, imageUrl, link } = props;
  const shouldRenderIcon = checkIfModuleOld(link);

  return (
    <StyledModuleTile>
      <div
        className="sub-title-content selectable"
        data-link={getLink(link)}
        onClick={onItemClick}
      >
        <div className="sub-title-image-container">
          <img className="sub-title-image" src={imageUrl} />
        </div>
        <Text fontSize="18px" className="sub-title-text">
          {title}
          {shouldRenderIcon && <StyledExternalLinkIcon color="#333333" />}
        </Text>
      </div>
    </StyledModuleTile>
  );
};

ModuleTile.propTypes = {
  title: PropTypes.string.isRequired,
  imageUrl: PropTypes.string.isRequired,
  link: PropTypes.string.isRequired,
};

export default ModuleTile;
