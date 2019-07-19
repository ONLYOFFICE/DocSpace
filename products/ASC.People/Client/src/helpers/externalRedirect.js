import React, { Component } from "react";

export class ExternalRedirect extends Component {
  constructor( props ){
    super();
    this.state = {
        location: props.to
    };
  }
  componentWillMount(){
    window.location.replace(this.state.location);
  }
  render(){
    return (<></>);
  }
}

export default ExternalRedirect;