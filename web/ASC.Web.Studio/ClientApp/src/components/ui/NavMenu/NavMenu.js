import React, { useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.scss';

const NavMenu = () => {
    const [mode, setMode] = useState({ isOpen: false });

    const toggle = () => {
        setMode({
            isOpen: !mode.isOpen
        });
    }

    return (
        <Container fluid className="p-0">
            <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-5" dark>
                <NavbarBrand tag={Link} to="/" className="product-menu">
                    <svg
                        height={23}
                        width={142}
                        xmlns="http://www.w3.org/2000/svg"
                        xmlnsXlink="http://www.w3.org/1999/xlink">
                        <image
                            xlinkHref="images/light_small_general.svg"
                        />
                    </svg>
                </NavbarBrand>
                <UncontrolledDropdown inNavbar size="sm">
                    <DropdownToggle caret nav className="product-menu with-subitem">
                        Choose
                        </DropdownToggle>
                    <DropdownMenu right>
                        <DropdownItem tag={Link} to="/products/files">Documents</DropdownItem>
                        <DropdownItem tag={Link} to="/products/projects">Projects</DropdownItem>
                        <DropdownItem tag={Link} to="/products/crm">CRM</DropdownItem>
                        <DropdownItem tag={Link} to="/products/mail"><i className="fa fa-envelope fa-fw"></i> Mail</DropdownItem>
                        <DropdownItem tag={Link} to="/products/community">Community</DropdownItem>
                        <DropdownItem tag={Link} to="/products/talk">Talk</DropdownItem>
                        <DropdownItem tag={Link} to="/products/calendar">Calendar</DropdownItem>
                        <DropdownItem tag={Link} to="/products/feed">Feed</DropdownItem>
                        <DropdownItem divider />
                        <DropdownItem tag={Link} to="/settings">Settings</DropdownItem>
                        <DropdownItem tag={Link} to="/services">Services</DropdownItem>
                        <DropdownItem tag={Link} to="/payments">Payments</DropdownItem>
                    </DropdownMenu>
                </UncontrolledDropdown>
                <NavbarToggler onClick={toggle} className="mr-2" />
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={mode.isOpen} navbar>
                    <UncontrolledDropdown inNavbar size="sm">
                        <DropdownToggle caret nav className="product-menu with-subitem">
                            User
                            </DropdownToggle>
                        <DropdownMenu right>
                            <DropdownItem tag={Link} to="/profile">Profile</DropdownItem>
                            <DropdownItem tag={Link} to="/about">About this program</DropdownItem>
                            <DropdownItem tag={Link} to="/logout">Sign Out</DropdownItem>
                        </DropdownMenu>
                    </UncontrolledDropdown>
                    <ul className="navbar-nav flex-grow">
                        <NavItem tag="li" className="top-item-box tariffs">
                            <NavLink tag={Link} to="/tariffs" title="Payments" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenupayments" />
                                </svg>
                            </NavLink>
                        </NavItem>
                        <NavItem tag="li" className="top-item-box settings">
                            <NavLink tag={Link} to="/settings" title="Settings" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenusettings" />
                                </svg>
                            </NavLink>
                        </NavItem>
                        <NavItem tag="li" className="top-item-box search">
                            <NavLink title="Search" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenusearch" />
                                </svg>
                            </NavLink>
                        </NavItem>
                        <NavItem tag="li" className="top-item-box feed">
                            <NavLink tag={Link} to="/feed" title="Feed" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenufeed" />
                                </svg>
                            </NavLink>
                        </NavItem>
                        <NavItem tag="li" className="top-item-box mail">
                            <NavLink tag={Link} to="/mail" title="Mail" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenumail" />
                                </svg>
                            </NavLink>
                        </NavItem>
                        <NavItem tag="li" className="top-item-box talk">
                            <NavLink tag={Link} to="/talk" title="Talk" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenuTalk" />
                                </svg>
                            </NavLink>
                        </NavItem>
                        <NavItem tag="li" className="top-item-box calendar">
                            <NavLink tag={Link} to="/calendar" title="Calendar" className="inner-text">
                                <svg viewBox="0 0 16 16" width={16} height={16}>
                                    <use xlinkHref="/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenuCalendar" />
                                </svg>
                            </NavLink>
                        </NavItem>
                    </ul>
                </Collapse>
            </Navbar>
        </Container>
    );
}

export default NavMenu;
