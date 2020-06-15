import React, { Component } from "react";
import PropTypes from "prop-types";
import StyledDragAndDrop from "./StyledDragAndDrop";

class DragAndDrop extends Component {
  state = { drag: false };

  dropRef = React.createRef();
  dragCounter = 0;

  onDragEnter = e => {
    this.dragCounter++;
    e.stopPropagation();
    e.preventDefault();

    this.props.onDragEnter && this.props.onDragEnter(e);
    if (e.dataTransfer.items.length) {
      this.setState({ drag: true });
    }
  };

  onDragLeave = e => {
    this.dragCounter--;

    if (this.dragCounter === 0) {
      this.setState({ drag: false });
      if (this.props.isDropZone) {
        return false;
      }
      this.props.onDragLeave && this.props.onDragLeave(e);
    }
    e.stopPropagation();
    e.preventDefault();
  };

  onDrop = e => {
    e.preventDefault();
    e.stopPropagation();
    this.setState({ drag: false });
    this.props.onDrop && this.props.onDrop(e);
    this.dragCounter = 0;
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      e.dataTransfer.clearData();
    }
  };

  componentDidMount() {
    let div = this.dropRef.current;
    div.addEventListener("drop", this.onDrop);
    div.addEventListener("dragenter", this.onDragEnter);
    div.addEventListener("dragleave", this.onDragLeave);
  }

  componentWillUnmount() {
    let div = this.dropRef.current;
    div.removeEventListener("drop", this.onDrop);
    div.removeEventListener("dragenter", this.onDragEnter);
    div.removeEventListener("dragleave", this.onDragLeave);
  }

  render() {
    const { children, dragging, isDropZone, className, ...rest } = this.props;
    const classNameProp = className ? className : "";

    return (
      <StyledDragAndDrop
        dragging={dragging}
        className={`drag-and-drop draggable${classNameProp}`}
        drag={this.state.drag && isDropZone}
        {...rest}
        ref={this.dropRef}
      >
        {children}
      </StyledDragAndDrop>
    );
  }
}

DragAndDrop.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  isDropZone: PropTypes.bool,
  dragging: PropTypes.bool,
  onDragEnter: PropTypes.func,
  onDragLeave: PropTypes.func,
  onDrop: PropTypes.func
};

DragAndDrop.defaultProps = {
  dragging: false
};

export default DragAndDrop;
