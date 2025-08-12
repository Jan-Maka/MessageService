import React, {createContext, useContext, useEffect, useState} from "react";

import PropTypes from "prop-types";

import { AuthContext } from "./AuthContext";
import {startConnection, stopConnection, setUpNotificationCallback} from "../services/signalRService";

const SocketContext = createContext(null);
export const useSocket = () => useContext(SocketContext);

export const SocketProvider = ({ children }) => {
    const { isLoggedIn } = useContext(AuthContext);
    const [isConnection, setIsConnected] = useState(false);

    useEffect(() => {
        const initializeSocket = async () => {
            if (isLoggedIn) {
                await startConnection().then(() => {
                    setIsConnected(true);
                });
            } else {
                await stopConnection().then(() => {
                    setIsConnected(false);
                });
            }
        }
        initializeSocket();
    }, [isLoggedIn]);

    return (
        <SocketContext.Provider value={{isConnection, setUpNotificationCallback}}>
            {children}
        </SocketContext.Provider>
    );
}

SocketProvider.propTypes = {
    children: PropTypes.node.isRequired,
};