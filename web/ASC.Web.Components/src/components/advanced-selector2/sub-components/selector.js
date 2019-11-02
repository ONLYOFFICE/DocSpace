import React from "react";
import PropTypes from "prop-types";
import styled, {css} from "styled-components";
import ADSelectorColumn from "./column";
import ADSelectorFooter from "./footer";
import ADSelectorHeader from "./header";
import ADSelectorBody from "./body";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    display: grid;

    ${props => props.displayType === "dropdown" ? css`
        grid-auto-rows: max-content;
        grid-template-areas: "column1 column2" "footer footer";

        .column2 { 
            grid-area: column2; 

            display: grid;
            background-color: gold;
            padding: 16px 16px 0 16px;

            grid-template-columns: 1fr;
            grid-template-rows: 64px 1fr;
            grid-template-areas: "header2" "body2";

            .header2 {
                grid-area: header2; 
                background-color: white;
            }

            .body2 {
                grid-area: body2;
                background-color: cyan;
                
            }
        }
    `
    : css`
        height: 100%;
        grid-template-columns: 1fr;
        grid-template-rows: 1fr 69px;
        grid-template-areas: "column1" "footer";
    `}   

    .column1 { 
        grid-area: column1;

        display: grid;
        background-color: red;
        padding: 16px 16px 0 16px;

        grid-template-columns: 1fr;
        grid-template-rows: 64px 1fr;
        grid-template-areas: "header1" "body1";

        .header1 {
            grid-area: header1; 
            background-color: white;
        }

        .body1 {
            grid-area: body1;
            background-color: lightblue;
        }
    }

    .footer { 
        grid-area: footer; 
    }
`;

class ADSelector extends React.Component {
    constructor(props) {
        super(props);
    }



    render() {
        const { displayType, groups, selectButtonLabel, isDisabled, isMultiSelect } = this.props;
        return (
            <StyledContainer displayType={displayType}>
                <ADSelectorColumn className="column1" displayType={displayType}>
                    <ADSelectorHeader className="header1">
                        <span>Header 1</span>    
                    </ADSelectorHeader>
                    <ADSelectorBody className="body1">
                        <span>Body 1</span>
                    </ADSelectorBody>
                </ADSelectorColumn>
                {displayType === "dropdown" && groups && groups.length > 0 &&
                    <ADSelectorColumn className="column2" displayType={displayType}>
                        <ADSelectorHeader className="header2">
                            <span>Header 2</span>    
                        </ADSelectorHeader>
                        <ADSelectorBody className="body2">
                            <span>Body 2</span>
                        </ADSelectorBody>
                    </ADSelectorColumn>
                }
                <ADSelectorFooter
                    className="footer"
                    selectButtonLabel={selectButtonLabel}
                    isDisabled={isDisabled}
                    isMultiSelect={isMultiSelect}
                    isVisible={true}
                />
            </StyledContainer>
        );
    }

}

ADSelector.propTypes = {
    options: PropTypes.array,
    groups: PropTypes.array,

    hasNextPage: PropTypes.bool,
    isNextPageLoading: PropTypes.bool,
    loadNextPage: PropTypes.func,

    isDisabled: PropTypes.bool,
    isMultiSelect: PropTypes.bool,

    selectButtonLabel: PropTypes.string,
    selectAllLabel: PropTypes.string,
    searchPlaceHolderLabel: PropTypes.string,

    //size: PropTypes.oneOf(["compact", "full"]),
    displayType: PropTypes.oneOf(["dropdown", "aside"]),

    selectedOptions: PropTypes.array,
    selectedGroups: PropTypes.array,

    onSelect: PropTypes.func,
    onSearchChanged: PropTypes.func
};

export default ADSelector;