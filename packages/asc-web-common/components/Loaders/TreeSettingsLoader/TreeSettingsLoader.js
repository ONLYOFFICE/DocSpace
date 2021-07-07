import React from "react";
import PropTypes from "prop-types";
import StyledTreeSettingsLoader from "./StyledTreeSettingsLoader";
import TreeNodeLoader from "../TreeNodeLoader";

const TreeSettingsLoader = ({ id, className, style, ...rest }) => {
  return (
    <div id={id} className={className} style={style}>
      <StyledTreeSettingsLoader>
        <TreeNodeLoader {...rest} />
      </StyledTreeSettingsLoader>
    </div>
  );
};

TreeSettingsLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

TreeSettingsLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default TreeSettingsLoader;
