import React, { useCallback } from "react";
import PropTypes from "prop-types";
import Text from "@appserver/components/text";
import StyledModuleTile from "./StyledModuleTile";
import { Link } from "react-router-dom";
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
            <div className="title-text">
              <Link to={`${link}`}>
                <Text fontSize="36px" className="title-text-header selectable">
                  {title}
                </Text>
                <Text fontSize="12px" className="title-text-description">
                  {description}
                </Text>
              </Link>
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
              <Link to={`${link}`}>
                <Text fontSize="18px" className="sub-title-text">
                  {title}
                </Text>
              </Link>
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
