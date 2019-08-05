import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import {
    MainButton,
    DropDownItem,
} from "asc-web-components";
import { isAdmin } from '../../../store/auth/selectors';

class ArticleMainButtonContent extends React.Component {
    onDropDownItemClick = (link) => {
        console.log("onDropDownItemClick", this, link);
        link && this.props.history.push(link);
    };

    render() {
        console.log("People ArticleMainButtonContent render");
        const { isAdmin, settings } = this.props;
        return (
            isAdmin ?
                <MainButton
                    isDisabled={false}
                    isDropdown={true}
                    text={"Actions"}
                >
                    <DropDownItem
                        label="New employee"
                        onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/create/user`)}
                    />
                    <DropDownItem
                        label="New guest"
                        onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/create/guest`)}
                    />
                    <DropDownItem
                        label="New department"
                        onClick={this.onDropDownItemClick}
                    />
                    <DropDownItem isSeparator />
                    <DropDownItem
                        label="Invitation link"
                        onClick={this.onDropDownItemClick}
                    />
                    <DropDownItem
                        label="Invite again"
                        onClick={this.onDropDownItemClick}
                    />
                    <DropDownItem
                        label="Import people"
                        onClick={this.onDropDownItemClick}
                    />
                </MainButton>
                :
                <></>
        );
    };
};

ArticleMainButtonContent.propTypes = {
    isAdmin: PropTypes.bool.isRequired,
    history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
    return {
        isAdmin: isAdmin(state.auth),
        settings: state.settings
    }
}

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));