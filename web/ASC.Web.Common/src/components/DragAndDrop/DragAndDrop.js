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
    //this.dragCounter++;
    this.props.onDragOver && this.props.onDragOver(e);
  };

  onDragEnter = (e) => {
    this.dragCounter++;
    e.stopPropagation();
    e.preventDefault();

    this.props.onDragEnter && this.props.onDragEnter(e);
    this.setState({ drag: true });
  };

  onDragLeave = (e) => {
    this.dragCounter--;

    if(this.dragCounter === 0) {
      this.setState({ drag: false });
    }
    this.props.onDragLeave && this.props.onDragLeave(e);
    e.stopPropagation();
    e.preventDefault();
  };

  onDrop = (e) => {
    e.preventDefault();
    e.stopPropagation();
    
    this.setState({ drag: false });
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      this.props.onDrop && this.props.onDrop(e);
      e.dataTransfer.clearData();
      this.dragCounter = 0;
    }
  };

  onDragStart = (e) => {
    this.props.onDragStart && this.props.onDragStart(e);
  };

  componentDidMount() {
    let div = this.dropRef.current;
    div.addEventListener("dragstart", this.onDragStart, false);
    div.addEventListener("dragenter", this.onDragEnter);
    div.addEventListener("dragleave", this.onDragLeave);
    div.addEventListener("dragover", this.onDragOver);
    div.addEventListener("drop", this.onDrop);
  }

  componentWillUnmount() {
    let div = this.dropRef.current;
    div.removeEventListener("dragstart", this.onDragStart, false);
    div.removeEventListener("dragenter", this.onDragEnter);
    div.removeEventListener("dragleave", this.onDragLeave);
    div.removeEventListener("dragover", this.onDragOver);
    div.removeEventListener("drop", this.onDrop);
  }

  render() {
    //console.log("DND render");
    const { children, draggable, ...rest } = this.props;
    return (
      <StyledDragAndDrop className="drag-and-drop" drag={this.state.drag} draggable={draggable} {...rest} ref={this.dropRef}>
        {children}
      </StyledDragAndDrop>
    );
  }
}

DragAndDrop.propTypes = {
  children: PropTypes.any,
  draggable: PropTypes.bool,
  onDragStart: PropTypes.func,
  onDragEnter: PropTypes.func,
  onDragLeave: PropTypes.func,
  onDragOver: PropTypes.func,
  onDrop: PropTypes.func
}

export default DragAndDrop;
