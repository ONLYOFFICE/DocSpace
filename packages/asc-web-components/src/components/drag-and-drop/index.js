import React from "react";
import { useDropzone } from "react-dropzone";
import PropTypes from "prop-types";

import StyledDragAndDrop from "./styled-drag-and-drop";

const DragAndDrop = (props) => {
  const { isDropZone, children, dragging, className, ...rest } = props;
  const classNameProp = className ? className : "";

  const onDrop = (acceptedFiles, array) => {
    acceptedFiles.length && props.onDrop && props.onDrop(acceptedFiles);
  };

  const { getRootProps, isDragActive } = useDropzone({
    noDragEventsBubbling: !isDropZone,
    onDrop,
  });

  return (
    <StyledDragAndDrop
      {...rest}
      className={`drag-and-drop ${classNameProp}`}
      dragging={dragging}
      isDragAccept={isDragActive}
      drag={isDragActive && isDropZone}
      {...getRootProps()}
    >
      {children}
    </StyledDragAndDrop>
  );
};

DragAndDrop.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  isDropZone: PropTypes.bool,
  dragging: PropTypes.bool,
  onMouseDown: PropTypes.func,
  onDrop: PropTypes.func,
};

export default DragAndDrop;
