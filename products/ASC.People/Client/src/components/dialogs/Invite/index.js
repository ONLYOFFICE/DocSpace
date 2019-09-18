import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import {
    toastr,
    ModalDialog,
    Link,
    Checkbox,
    Button,
    Textarea,
    Text
} from "asc-web-components";
import { getInvitationLink, getShortenedLink } from '../../../store/profile/actions';
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from './i18n';
import { typeGuests } from './../../../helpers/customNames';
import styled from 'styled-components'

const ModalDialogContainer = styled.div`
    .margin-text {
        margin: 12px 0;
    }

    .margin-link {
        margin-right: 12px;
    }

    .margin-textarea {
        margin-top: 12px;
    }

    .flex{
        display: flex;
        justify-content: space-between;
    }
`;

const textAreaName = 'link-textarea';

class PureInviteDialog extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isGuest: false,
            peopleInvitationLink: '',
            guestInvitationLink: '',
            isLoading: true,
            isLinkShort: false
        }
    }

    onCopyLinkToClipboard = () => {
        const { t } = this.props;
        const link = document.getElementsByName(textAreaName)[0];
        link.select();
        document.execCommand('copy');
        toastr.success(t('LinkCopySuccess'));
        window.getSelection().removeAllRanges();
        link.blur();
    };

    onCheckedGuest = () => this.setState({ isGuest: !this.state.isGuest });

    onGetShortenedLink = () => {
        this.setState({ isLoading: true });
        const { getShortenedLink } = this.props;

        getShortenedLink(this.state.peopleInvitationLink)
            .then((res) => {
                // console.log("getShortInvitationLinkPeople success", res.data.response);
                this.setState({ peopleInvitationLink: res.data.response });
            })
            .catch(e => {
                console.error("getShortInvitationLink error", e);
                this.setState({ isLoading: false });
            });

        getShortenedLink(this.state.guestInvitationLink)
            .then((res) => {
                // console.log("getShortInvitationLinkGuest success", res.data.response);
                this.setState({
                    guestInvitationLink: res.data.response,
                    isLoading: false,
                    isLinkShort: true
                });
            })
            .catch(e => {
                console.error("getShortInvitationLink error", e);
            });

    };

    componentDidUpdate(prevProps, prevState) {
        if (!prevProps.visible && !prevState.peopleInvitationLink && !prevState.guestInvitationLink) {
            // console.log('INVITE DIALOG DidUpdate');
            const { getInvitationLink } = this.props;
            const isGuest = true;

            getInvitationLink()
                .then((res) => {
                    // console.log("getInvitationLinkPeople success", res.data.response);
                    this.setState({
                        peopleInvitationLink: res.data.response,
                        isLoading: false
                    });
                })
                .catch(e => {
                    console.error("getInvitationLinkPeople error", e);
                    this.setState({ isLoading: false });
                });

            getInvitationLink(isGuest)
                .then((res) => {
                    // console.log("getInvitationLinkGuest success", res.data.response);
                    this.setState({ guestInvitationLink: res.data.response });
                })
                .catch(e => {
                    console.error("getInvitationLinkGuest error", e);
                    this.setState({ isLoading: false });
                });
        };
    }

    onClickToCloseButton = () => this.props.onCloseButton && this.props.onCloseButton();

    render() {
        console.log("InviteDialog render");
        const { t, visible, onClose } = this.props;
        const fakeSettings = { hasShortenService: false };

        return (
            <ModalDialogContainer>
                <ModalDialog
                    visible={visible}
                    onClose={() => onClose && onClose()}

                    headerContent={t('InviteLinkTitle')}

                    bodyContent={(
                        <>
                            <Text.Body
                                className='margin-text'
                                as='p'>
                                {t('HelpAnswerLinkInviteSettings')}
                            </Text.Body>
                            <Text.Body
                                className='margin-text'
                                as='p'>
                                {t('InviteLinkValidInterval', { count: 7 })}
                            </Text.Body>
                            <div className='flex'>
                                <div>
                                    <Link
                                        className='margin-link'
                                        type='action'
                                        isHovered={true}
                                        onClick={this.onCopyLinkToClipboard}
                                    >
                                        {t('CopyToClipboard')}
                                    </Link>
                                    {
                                        fakeSettings.hasShortenService && !this.state.isLinkShort &&
                                        <Link type='action'
                                            isHovered={true}
                                            onClick={this.onGetShortenedLink}
                                        >
                                            {t('GetShortenLink')}
                                        </Link>
                                    }
                                </div>
                                <Checkbox
                                    label={t('InviteUsersAsCollaborators', { typeGuests })}
                                    isChecked={this.state.isGuest}
                                    onChange={this.onCheckedGuest}
                                />
                            </div>
                            <Textarea
                                className='margin-textarea'
                                isReadOnly={true}
                                isDisabled={this.state.isLoading}
                                name={textAreaName}
                                value={this.state.isGuest ? this.state.guestInvitationLink : this.state.peopleInvitationLink}
                            />
                        </>
                    )}

                    footerContent={(
                        <>
                            <Button
                                key="CloseBtn"
                                label={this.state.isLoading ? t('LoadingProcessing') : t('CloseButton')}
                                size="medium"
                                primary={true}
                                onClick={this.onClickToCloseButton}
                                isLoading={this.state.isLoading}
                            />
                        </>
                    )}
                />
            </ModalDialogContainer>
        );
    };
};


const mapStateToProps = (state) => {
    return {
        settings: state.auth.settings
    }
}

const InviteDialogContainer = withTranslation()(PureInviteDialog);

const InviteDialog = (props) => <I18nextProvider i18n={i18n}><InviteDialogContainer {...props} /></I18nextProvider>;

InviteDialog.propTypes = {
    visible: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    onCloseButton: PropTypes.func.isRequired
};

export default connect(mapStateToProps, { getInvitationLink, getShortenedLink })(withRouter(InviteDialog));