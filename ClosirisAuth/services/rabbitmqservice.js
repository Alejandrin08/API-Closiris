const amqp = require('amqplib');

async function sendAuditMessage(action, userId) {
    try {
        const connection = await amqp.connect('amqp://localhost'); 
        const channel = await connection.createChannel();
        const queue = 'closiris_logs'; 
        await channel.assertQueue(queue, { durable: false });

        const message = {
            action: action,
            userId: userId
        };

        channel.sendToQueue(queue, Buffer.from(JSON.stringify(message)));
        console.log(`[x] Sent ${JSON.stringify(message)}`);

        setTimeout(() => {
            connection.close();
        }, 500); 
    } catch (error) {
        console.error('Error sending audit message:', error);
    }
}

module.exports = { sendAuditMessage };
