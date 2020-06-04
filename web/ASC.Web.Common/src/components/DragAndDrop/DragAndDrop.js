import React, { Component } from "react";
import PropTypes from "prop-types";
import StyledDragAndDrop from "./StyledDragAndDrop";

class DragAndDrop extends Component {
  state = { drag: false };

  dropRef = React.createRef();
  dragCounter = 0;

  onDragOver = (e) => {
    e.stopPropagation();
    e.preventDefault();
    this.props.onDragOver && this.props.onDragOver(e);
    if(e.dataTransfer.items.length) {
      this.setState({ drag: true });
    }
  };

  onDragEnter = (e) => {
    this.dragCounter++;
    e.stopPropagation();
    e.preventDefault();

    this.props.onDragEnter && this.props.onDragEnter(e);
    if(e.dataTransfer.items.length) {
      this.setState({ drag: true });
    }
  };

  onDragLeave = (e) => {
    this.dragCounter--;

    if(this.dragCounter === 0) {
      this.setState({ drag: false });
      this.props.onDragLeave && this.props.onDragLeave(e);
    }
    e.stopPropagation();
    e.preventDefault();
  };

  onDrop = (e) => {
    e.preventDefault();
    e.stopPropagation();
    
    this.setState({ drag: false });
    this.props.onDrop && this.props.onDrop(e);
    this.dragCounter = 0;
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      e.dataTransfer.clearData();
    }
  };

  onDragStart = (e) => {
    this.props.onDragStart && this.props.onDragStart(e);
  };

  onDragEnd = (e) => {
    this.props.onDragEnd && this.props.onDragEnd(e);
  }

  componentDidMount() {
    let div = this.dropRef.current;
    div.addEventListener("dragstart", this.onDragStart, false);
    div.addEventListener("dragend", this.onDragEnd, false);
    div.addEventListener("dragenter", this.onDragEnter);
    div.addEventListener("dragleave", this.onDragLeave);
    div.addEventListener("dragover", this.onDragOver);
    div.addEventListener("drop", this.onDrop);
  }

  componentWillUnmount() {
    let div = this.dropRef.current;
    div.removeEventListener("dragstart", this.onDragStart, false);
    div.removeEventListener("dragend", this.onDragEnd, false);
    div.removeEventListener("dragenter", this.onDragEnter);
    div.removeEventListener("dragleave", this.onDragLeave);
    div.removeEventListener("dragover", this.onDragOver);
    div.removeEventListener("drop", this.onDrop);
  }

  shouldComponentUpdate(nextProps, nextState) {
    const { draggable, dragging } = this.props;

    if(draggable !== nextProps.draggable) {
      return true;
    }

    if(dragging !== nextProps.dragging) {
      return true;
    }

    if(this.state.drag !== nextState.drag) {
      return true;
    }

    return false;
  }

  render() {
    const { children, draggable, dragging, isDropZone, ...rest } = this.props;

    return (
      <StyledDragAndDrop
        dragging={dragging}
        className="drag-and-drop"
        drag={this.state.drag && isDropZone}
        draggable={draggable}
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
  draggable: PropTypes.bool,
  isDropZone: PropTypes.bool,
  dragging: PropTypes.bool,
  onDragStart: PropTypes.func,
  onDragEnd: PropTypes.func,
  onDragEnter: PropTypes.func,
  onDragLeave: PropTypes.func,
  onDragOver: PropTypes.func,
  onDrop: PropTypes.func
}

DragAndDrop.defaultProps = {
  dragging: false
}

export default DragAndDrop;
