import React from "react";
import PropTypes from "prop-types";
import { StyledTreeFolder, StyledLoader } from "./StyledTreeFolder";
import TreeNodeLoader from "../TreeNodeLoader";

const NewTreeFolderLoader = ({ id, className, style, ...rest }) => {
  return (
    <StyledTreeFolder id={id} className={className} style={style}>
      <StyledLoader>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>
      <StyledLoader paddingLeft={"16px"}>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>

      <StyledLoader paddingLeft={"32px"}>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>
      <StyledLoader paddingLeft={"32px"}>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>
      <StyledLoader paddingLeft={"32px"}>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>

      <StyledLoader>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>
      <StyledLoader paddingLeft={"16px"}>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>
      <StyledLoader paddingLeft={"16px"}>
        <TreeNodeLoader {...rest} withRectangle />
      </StyledLoader>
    </StyledTreeFolder>
  );
};

NewTreeFolderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

NewTreeFolderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default NewTreeFolderLoader;
