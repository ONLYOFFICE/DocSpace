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

  const onDragOver = (e) => {
    props.onDragOver && props.onDragOver(isDragActive, e);
  };

  const onDragLeave = (e) => {
    props.onDragLeave && props.onDragLeave(e);
  };

  const { getRootProps, isDragActive } = useDropzone({
    noDragEventsBubbling: !isDropZone,
    onDrop,
    onDragOver,
    onDragLeave,
  });

  return (
    <StyledDragAndDrop
      {...rest}
      className={`drag-and-drop ${classNameProp}`}
      dragging={dragging}
      isDragAccept={isDragActive}
      drag={isDragActive && isDropZone && props.onDrop}
      {...getRootProps()}
    >
      {children}
    </StyledDragAndDrop>
  );
};

DragAndDrop.propTypes = {
  /** Children elements */
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Sets the component as a dropzone */
  isDropZone: PropTypes.bool,
  /** Show that the item is being dragged now. */
  dragging: PropTypes.bool,
  /** Occurs when the mouse button is pressed */
  onMouseDown: PropTypes.func,
  /** Occurs when the dragged element is dropped on the drop target */
  onDrop: PropTypes.func,
  onDragOver: PropTypes.func,
  onDragLeave: PropTypes.func,
};

export default DragAndDrop;
