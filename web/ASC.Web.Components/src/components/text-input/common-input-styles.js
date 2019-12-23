import {css} from 'styled-components';

const commonInputStyle = css`
    width: ${props =>
        (props.scale && '100%') ||
        (props.size === 'base' && '173px') ||
        (props.size === 'middle' && '300px') ||
        (props.size === 'big' && '350px') ||
        (props.size === 'huge' && '500px')
    };
    background-color: ${props => props.isDisabled ? '#F8F9F9' : '#fff'};
    color: ${props => props.isDisabled ? '#D0D5DA' : '#333333'};

    border-radius: 3px;
    box-shadow: none;
    box-sizing: border-box;
    border: solid 1px;
    border-color: ${props => (props.hasError && '#c30') || (props.hasWarning && '#f1ca92') || (props.isDisabled && '#ECEEF1')|| '#D0D5DA'};
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;

    :hover{
        border-color: ${props => (props.hasError && '#c30') || (props.hasWarning && '#f1ca92') || (props.isDisabled && '#ECEEF1')|| '#A3A9AE'};
    }
    :focus{
        border-color: ${props => (props.hasError && '#c30') || (props.hasWarning && '#f1ca92') || (props.isDisabled && '#ECEEF1')|| '#2DA7DB'};
    }

    cursor: ${props => props.isReadOnly || props.isDisabled ? "default" : "text"};
`;

export default commonInputStyle;