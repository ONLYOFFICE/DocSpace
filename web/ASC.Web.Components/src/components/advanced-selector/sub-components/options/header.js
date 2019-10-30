import React from "react";
import PropTypes from "prop-types";
import SearchInput from "../../../search-input";
import Button from "../../../button";
import { Icons } from "../../../icons";

const ADSelectorOptionsHeader = (props) => {
    const { searchPlaceHolder, value, isDisabled, allowCreation, onSearchChanged, onAddNewClick } = props;

    return (
        <div className="head_container">
            <SearchInput
                className="options_searcher"
                isDisabled={isDisabled}
                size="base"
                scale={true}
                isNeedFilter={false}
                placeholder={searchPlaceHolder}
                value={value}
                onChange={onSearchChanged}
                onClearSearch={onSearchChanged}
            />
            {allowCreation && (
                <Button
                    className="add_new_btn"
                    primary={false}
                    size="base"
                    label=""
                    icon={
                        <Icons.PlusIcon
                            size="medium"
                            isfill={true}
                            color="#D8D8D8"
                        />
                    }
                    onClick={onAddNewClick}
                />
            )}
        </div>
    );
};

ADSelectorOptionsHeader.propTypes = {
    searchPlaceHolder: PropTypes.string,
    value: PropTypes.string,
    isDisabled: PropTypes.bool,
    allowCreation: PropTypes.bool,
    style: PropTypes.object,
    onSearchChanged: PropTypes.func,
    onAddNewClick: PropTypes.func
}

export default ADSelectorOptionsHeader;