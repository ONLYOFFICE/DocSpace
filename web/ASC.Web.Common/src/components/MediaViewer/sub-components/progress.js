import React from 'react'
import styled from "styled-components";

const StyledProgress = styled.div`
  display: inline-block;

  .slider-container{
    display:inline-block;
    border-radius: 2px;
    position:relative;	
    width: ${props => props.width}px;
    height:6px;
    background:rgba(200,200,200,0.2);
    margin:15px 0;
  }
  .fill{
    cursor: pointer;
    width: ${props => 100 * props.value}%;
    position:absolute;
    
    top:calc(50% - 3px);
    height:6px;
    background: #d1d1d1;
    border-radius: 2px;
    
  }
  input[type='range'] {
      display: block;
      overflow: visible;
      background: transparent;
      width: ${props => props.width}px;
      height:6px;
      outline:none;
      margin:0;
      -webkit-appearance: none;
      position:relative;
      cursor: pointer;
  }

  input[type='range']::-webkit-slider-thumb {
      position:relative;
      appearance: none;
	    box-sizing: content-box;
      width: 12px;
      height: 12px;
      margin-top: -3px;
      background: white;
      border-radius: 50%;
      
      cursor: pointer;
      
  }
  input[type=range]::-moz-range-thumb {
      position:relative;
      appearance: none;
	    box-sizing: content-box;
      width: 12px;
      height: 12px;
      background: white;
      border-radius: 50%;
      margin-top: -3px;
      cursor: pointer;
     
  }
  input[type=range]::-ms-thumb {
      position:relative;
      appearance: none;
	    box-sizing: content-box;
      width: 12px;
      height: 12px;
      background: white;
      border-radius: 50%;
      margin-top: -3px;
      cursor: pointer;
      
  }

  input[type='range']::-webkit-slider-runnable-track {
    margin: 12px 0;
    height: 6px;
    border-radius: 2px;
    cursor: pointer;
    -webkit-appearance: none;
    text-align: right;
    pointer-events: none;
    
  }
  input[type="range"]::-moz-range-track  {
    margin: 12px 0;
    height: 6px;
    border-radius: 2px;
    cursor: pointer;
    -webkit-appearance: none;
    text-align: right;
    pointer-events: none;
    
  }
  input[type=range]::-ms-track { 
    border-color: transparent;
    color: transparent;
    
    margin: 12px 0;
    height: 6px;
    border-radius: 2px;
    cursor: pointer;
    -webkit-appearance: none;
    text-align: right;
    pointer-events: none;
    
  }
`;

class Progress extends React.Component {

    constructor(props) {
        super(props);
    }

    render(){
        return (
            <StyledProgress {...this.props} >
                <div className="slider-container">
                <div className="fill"></div>
                <input
                    type='range' min={0} max={0.999999} step='any'
                    value={this.props.value}
                    onMouseDown={this.props.handleSeekMouseDown}
                    onChange={this.props.handleSeekChange}
                    onMouseUp={this.props.handleSeekMouseUp}
                />
                </div>
            </StyledProgress>
        );
    }
}

Progress.propTypes = {}

Progress.defaultProps = {}

export default Progress;