import React from 'react';
import { connect } from 'react-redux';
import {
    MainButton,
    DropDownItem,
} from "asc-web-components";

const ArticleMainButtonContent = ({ isAdmin }) => (
    isAdmin ?
        <MainButton
            isDisabled={false}
            isDropdown={true}
            text={"Actions"}
            clickAction={() => console.log("MainButton clickAction")}
        >
            <DropDownItem
                label="New employee"
                onClick={() => console.log("New employee clicked")}
            />
            <DropDownItem
                label="New quest"
                onClick={() => console.log("New quest clicked")}
            />
            <DropDownItem
                label="New department"
                onClick={() => console.log("New department clicked")}
            />
            <DropDownItem isSeparator />
            <DropDownItem
                label="Invitation link"
                onClick={() => console.log("Invitation link clicked")}
            />
            <DropDownItem
                label="Invite again"
                onClick={() => console.log("Invite again clicked")}
            />
            <DropDownItem
                label="Import people"
                onClick={() => console.log("Import people clicked")}
            />
        </MainButton>
        :
        <></>
);

const mapStateToProps = (state) => {
    return {
        isAdmin: state.auth.user.isAdmin || state.auth.user.isOwner
    }
}

export default connect(mapStateToProps)(ArticleMainButtonContent);