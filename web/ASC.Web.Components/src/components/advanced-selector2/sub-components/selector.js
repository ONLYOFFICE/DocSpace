import React from "react";
import PropTypes from "prop-types";
import styled, {css} from "styled-components";
import ADSelectorColumn from "./column";
import ADSelectorFooter from "./footer";

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
            background-color: gold;
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
        background-color: red;
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
                    <span>Column 1</span>
                </ADSelectorColumn>
                {displayType === "dropdown" && groups && groups.length > 0 &&
                    <ADSelectorColumn className="column2" displayType={displayType}>
                        <span>Column 2</span>
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