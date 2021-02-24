import React, { useCallback } from "react";
import PropTypes from "prop-types";
import Text from "@appserver/components/src/components/text";
import StyledModuleTile from "./StyledModuleTile";

const ModuleTile = (props) => {
  // console.log("ModuleTile render", props);
  const { title, imageUrl, link, description, isPrimary, onClick } = props;

  const handleClick = useCallback(
    (e) => {
      if (typeof onClick !== "function") return;

      onClick(e, link);
    },
    [link, onClick]
  );
console.log("imageUrl", imageUrl)
  return (
    <StyledModuleTile>
      {isPrimary ? (
        <div className="title-content">
          <div className="title-image-wrapper">
            <img
              className="title-image selectable"
              src={imageUrl}
              onClick={handleClick}
            />
          </div>

          <div className="title-text-wrapper">
            <div onClick={handleClick} className="title-text">
              <Text fontSize="36px" className="title-text-header selectable">
                {title}
              </Text>
              <Text fontSize="12px" className="title-text-description">
                {description}
              </Text>
            </div>
          </div>
        </div>
      ) : (
        <div className="sub-title-content selectable">
          <div>
            <img
              className="sub-title-image"
              src={imageUrl}
              onClick={handleClick}
            />
          </div>
          <div>
            <div>
              <Text
                fontSize="18px"
                className="sub-title-text"
                onClick={handleClick}
              >
                {title}
              </Text>
            </div>
          </div>
        </div>
      )}
    </StyledModuleTile>
  );
};

ModuleTile.propTypes = {
  title: PropTypes.string.isRequired,
  imageUrl: PropTypes.string.isRequired,
  link: PropTypes.string.isRequired,
  description: PropTypes.string,
  isPrimary: PropTypes.bool,
  onClick: PropTypes.func.isRequired,
};

ModuleTile.defaultProps = {
  isPrimary: false,
  description: "",
};

export default ModuleTile;
