import React from 'react'
import styled from "styled-components";

const StyledVideoControlBtn = styled.div`
    display: inline-block;
    height: 30px;
    line-height: 25px;
    margin: 5px;
    width: 40px;
    border-radius: 2px;
    cursor: pointer;
    text-align: center;

    &:hover{
        background-color: rgba(200,200,200,0.2);
    }
`;

class ControlBtn extends React.Component {

    constructor(props) {
        super(props);
    }

    render(){
        return (
            <StyledVideoControlBtn {...this.props} >
                {this.props.children}
            </StyledVideoControlBtn>
        );
    }
}

ControlBtn.propTypes = {}

ControlBtn.defaultProps = {}

export default ControlBtn;