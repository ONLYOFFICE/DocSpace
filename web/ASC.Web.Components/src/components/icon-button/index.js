import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import { Icons } from '../icons';

const StyledOuter = styled.div`
    width: ${props => props.size ? Math.abs(parseInt(props.size)) + "px" : "20px"};
    cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
    line-height: 0;
`;
class IconButton extends React.Component{
    constructor(props) {
        super(props);
    
        this.state = {
            currentIconName: this.props.iconName,
            currentIconColor: this.props.color
        };
        this.onMouseEnter = this.onMouseEnter.bind(this);
        this.onMouseLeave = this.onMouseLeave.bind(this);
        this.onMouseDown = this.onMouseDown.bind(this);
        this.onMouseUp = this.onMouseUp.bind(this);
    }

   
    onMouseEnter(e){
        if(!this.props.isDisabled){
            this.setState({
                currentIconName: this.props.iconHoverName ? this.props.iconHoverName : this.props.iconName,
                currentIconColor: this.props.hoverColor ? this.props.hoverColor : this.props.color
            });
            this.props.onMouseEnter && this.props.onMouseEnter(e);
        }
    }
    onMouseLeave(e){
        if(!this.props.isDisabled){
            this.setState({
                currentIconName: this.props.iconName,
                currentIconColor: this.props.color
            });
            this.props.onMouseDown && this.props.onMouseDown(e);
        }
    }
    onMouseDown(e){
        if(!this.props.isDisabled){
            this.setState({
                currentIconName: this.props.iconClickName ? this.props.iconClickName : this.props.iconName,
                currentIconColor: this.props.clickColor ? this.props.clickColor :  this.props.color
            });
            this.props.onMouseDown && this.props.onMouseDown(e);
        }
    }
    onMouseUp(e){
        if(!this.props.isDisabled){
            switch (e.nativeEvent.which) {
                case 1: //Left click
                    this.setState({
                        currentIconName: this.props.iconHoverName ? this.props.iconHoverName : this.props.iconName,
                        currentIconColor: this.props.iconHoverName ? this.props.iconHoverName :  this.props.color
                    });
                    this.props.onClick && this.props.onClick(e);
                    this.props.onMouseUp && this.props.onMouseUp(e);
                    break;
                case 3://Right click
                    this.props.onMouseUp && this.props.onMouseUp(e);
                    break;
            
                default:
                    break;
                }
        }
    }
    render(){
        //console.log("IconButton render");
        return (
            <StyledOuter 
                size={this.props.size} 
                isDisabled={this.props.isDisabled} 
    
                onMouseEnter={this.onMouseEnter}
                onMouseLeave={this.onMouseLeave}
                onMouseDown={this.onMouseDown}
                onMouseUp={this.onMouseUp}
            >
                {React.createElement(Icons[this.state.currentIconName], {size: "scale", color: this.state.currentIconColor, isfill: this.props.isFill})}
            </StyledOuter>
        );
    };
};

IconButton.propTypes = {
    color: PropTypes.string,
    hoverColor: PropTypes.string,
    clickColor: PropTypes.string,
    size: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
    isFill: PropTypes.bool,
    isDisabled: PropTypes.bool,
    iconName: PropTypes.string.isRequired,
    iconHoverName: PropTypes.string,
    iconClickName: PropTypes.string,
    onClick:PropTypes.func
};

IconButton.defaultProps = {
    color: "#d0d5da",
    size: 25,
    isFill: true,
    iconName: "AZSortingIcon",
    isDisabled: false
};

export default IconButton;